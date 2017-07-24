using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.TokenDefinitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GT.Provisioning.Core.ExtensibilityProviders
{
    public class MigrateLibraryContentExtensibilityHandler : IProvisioningExtensibilityHandler
    {
        public ProvisioningTemplate Extract(ClientContext ctx
            , ProvisioningTemplate template
            , ProvisioningTemplateCreationInformation creationInformation
            , PnPMonitoredScope scope
            , string configurationData)
        {
            return template;
        }

        public IEnumerable<TokenDefinition> GetTokens(ClientContext ctx
            , ProvisioningTemplate template
            , string configurationData)
        {
            return new List<TokenDefinition>();
        }

        public void Provision(ClientContext ctx
            , ProvisioningTemplate template
            , ProvisioningTemplateApplyingInformation applyingInformation
            , TokenParser tokenParser
            , PnPMonitoredScope scope
            , string configurationData)
        {
            if (string.IsNullOrEmpty(configurationData))
            {
                return;
            }

            // parse xml
            var migrateXmlElement = ParseXml(configurationData);
            var migrateLists = from l in migrateXmlElement.Descendants("List")
                               select new
                               {
                                   Url = l.Attribute("Url").Value,
                                   sourceUrl = l.Attribute("Source").Value,
                                   destWebUrl = l.Attribute("Destination").Value
                               };

            foreach (var list in migrateLists)
            {
                string listUrl = tokenParser.ParseString(list.Url);
                string destWebRelativeUrl = tokenParser.ParseString(list.destWebUrl);
                string sourceWebRelativeUrl = tokenParser.ParseString(list.sourceUrl);

                MigrateListItems(ctx
                    , listUrl
                    , sourceWebRelativeUrl
                    , destWebRelativeUrl);
            }
        }

        private XElement ParseXml(string xmlString)
        {
            byte[] encodedString = Encoding.UTF8.GetBytes(xmlString);
            MemoryStream sourceStream = new MemoryStream(encodedString);
            sourceStream.Flush();
            sourceStream.Position = 0;

            // remove xml namespace
            XElement xElement = XElement.Load(sourceStream);
            foreach (XElement XE in xElement.DescendantsAndSelf())
            {
                XE.Name = XE.Name.LocalName;
                XE.ReplaceAttributes(
                    (from xattrib in XE.Attributes()
                     .Where(xa => !xa.IsNamespaceDeclaration)
                     select new XAttribute(xattrib.Name.LocalName, xattrib.Value))
                     );
            }

            return xElement;
        }

        private void MigrateListItems(ClientContext clientContext
            , string listUrl
            , string sourceWebUrl
            , string destinationWebUrl)
        {
            var siteCollectionContext = clientContext.GetSiteCollectionContext();
            var sourceWeb = siteCollectionContext.Site.OpenWeb(sourceWebUrl);
            var destinationWeb = siteCollectionContext.Site.OpenWeb(destinationWebUrl);

            // Get source list
            var sourceClientContext = sourceWeb.Context;
            var sourceList = sourceWeb.GetListByUrl(listUrl);
            sourceClientContext.Load(sourceList, l => l.Fields, l => l.BaseType);
            sourceClientContext.ExecuteQuery();

            var sourceListItems = sourceList.GetItems(CamlQuery.CreateAllItemsQuery());
            sourceClientContext.Load(sourceListItems);
            sourceClientContext.ExecuteQueryRetry();

            // get destination list
            var destinationClientContext = destinationWeb.Context;
            var destinationList = destinationWeb.GetListByUrl(listUrl);
            destinationClientContext.Load(destinationList);

            foreach (var listItem in sourceListItems)
            {
                switch (listItem.FileSystemObjectType)
                {
                    case FileSystemObjectType.Invalid:
                        break;
                    case FileSystemObjectType.File:
                        ProcessListItem(listItem, sourceList.Fields, sourceWeb, destinationWeb, destinationList);
                        break;
                    case FileSystemObjectType.Folder:
                        EnsureFolder(listItem, sourceWeb, destinationWeb);
                        break;
                    case FileSystemObjectType.Web:
                        break;
                    default:
                        break;
                }
            }
        }

        private Microsoft.SharePoint.Client.Folder EnsureFolder(ListItem listItem
            , Web sourceWeb
            , Web destinationWeb)
        {
            Microsoft.SharePoint.Client.Folder parentFolder = listItem.Folder;
            sourceWeb.Context.Load(parentFolder, f => f.Name, f => f.ServerRelativeUrl);
            sourceWeb.Context.ExecuteQueryRetry();

            if (parentFolder.IsPropertyAvailable("ServerRelativeUrl"))
            {
                var parentFolderPath = parentFolder.ServerRelativeUrl.Replace(sourceWeb.ServerRelativeUrl, string.Empty);

                var newFolder = destinationWeb.EnsureFolderPath(parentFolderPath, f => f.ServerRelativeUrl);
                destinationWeb.Context.Load(newFolder);
                destinationWeb.Context.ExecuteQueryRetry();

                return newFolder;
            }

            return null;
        }

        private void ProcessListItem(ListItem listItem
            , Microsoft.SharePoint.Client.FieldCollection fields
            , Web sourceWeb
            , Web destinationWeb
            , List destinationList)
        {
            switch (destinationList.BaseType)
            {
                case BaseType.None:
                    break;
                case BaseType.GenericList:
                    CopyItems(listItem, fields, sourceWeb, destinationWeb, destinationList);
                    break;
                case BaseType.DocumentLibrary:
                    CopyFiles(listItem, fields, sourceWeb, destinationWeb, destinationList);
                    break;
                case BaseType.Unused:
                    break;
                case BaseType.DiscussionBoard:
                    break;
                case BaseType.Survey:
                    break;
                case BaseType.Issue:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// copy files
        /// </summary>
        /// <param name="sourceListItem"></param>
        /// <param name="sourceFields"></param>
        /// <param name="sourceWeb"></param>
        /// <param name="destinationWeb"></param>
        /// <param name="destinationList"></param>
        private void CopyFiles(ListItem sourceListItem
            , Microsoft.SharePoint.Client.FieldCollection sourceFields
            , Web sourceWeb
            , Web destinationWeb
            , List destinationList)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("CopyFiles"))
            {
                sourceWeb.Context.Load(sourceListItem);
                sourceWeb.Context.ExecuteQueryRetry();

                try
                {
                    // get folder reference
                    if (sourceListItem["FileDirRef"] != null)
                    {
                        var folderUrl = sourceListItem["FileDirRef"].ToString();
                        folderUrl = folderUrl.Replace(sourceWeb.ServerRelativeUrl, destinationWeb.ServerRelativeUrl);

                        var destinationFolder = destinationWeb.GetFolderByServerRelativeUrl(folderUrl);

                        // get file name
                        var fileName = sourceListItem["FileLeafRef"].ToString();

                        // get file stream
                        var streamResult = sourceListItem.File.OpenBinaryStream();
                        sourceWeb.Context.ExecuteQueryRetry();
                                                
                        var uploadedFile = destinationFolder.UploadFile(fileName: fileName, stream: streamResult.Value, overwriteIfExists: true);

                        // update uploaded file properties
                        ListItem destinationListItem = uploadedFile.ListItemAllFields;
                        foreach (Microsoft.SharePoint.Client.Field field in sourceFields)
                        {
                            if (!field.ReadOnlyField
                                && !field.Hidden
                                && (field.InternalName != "Attachments")
                                && (field.InternalName != "ContentType"))
                            {
                                if (destinationList.FieldExistsByName(field.InternalName))
                                {
                                    destinationListItem[field.InternalName] = sourceListItem[field.InternalName];
                                    destinationListItem.Update();
                                }
                            }
                        }

                        destinationWeb.Context.ExecuteQueryRetry();
                    }
                }
                catch (Exception exception)
                {
                    Log.LogDebug(exception, exception.Message);
                }
            }
        }

        /// <summary>
        /// Copy list items
        /// </summary>
        /// <param name="sourceListItem">List item to be copied to destination lisst</param>
        /// <param name="sourceFields">Fields collection</param>
        /// <param name="destinationList">destination list where item to be copied</param>
        private void CopyItems(ListItem sourceListItem
            , Microsoft.SharePoint.Client.FieldCollection sourceFields
            , Web sourceWeb
            , Web destinationWeb
            , List destinationList)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("CopyItems"))
            {
                sourceWeb.Context.Load(sourceListItem);
                sourceWeb.Context.ExecuteQueryRetry();

                try
                {
                    // get folder reference
                    if (sourceListItem["FileDirRef"] != null)
                    {
                        var folderUrl = sourceListItem["FileDirRef"].ToString();

                        // update web url
                        folderUrl = folderUrl.Replace(sourceWeb.ServerRelativeUrl, destinationWeb.ServerRelativeUrl);

                        // set folderUrl path to create list item inside folder.
                        var destinationListItem = destinationList.AddItem(new ListItemCreationInformation()
                        {
                            FolderUrl = folderUrl
                        });

                        foreach (Microsoft.SharePoint.Client.Field field in sourceFields)
                        {
                            if (!field.ReadOnlyField
                                && !field.Hidden
                                && (field.InternalName != "Attachments")
                                && (field.InternalName != "ContentType"))
                            {
                                if (destinationList.FieldExistsByName(field.InternalName))
                                {
                                    destinationListItem[field.InternalName] = sourceListItem[field.InternalName];
                                    destinationListItem.Update();
                                }
                            }
                        }

                        destinationWeb.Context.ExecuteQueryRetry();
                    }
                }
                catch (Exception exception)
                {
                    Log.LogDebug(exception, exception.Message);
                }
            }
        }
    }
}