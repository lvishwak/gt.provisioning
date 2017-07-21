using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.TokenDefinitions;
using System.Xml.Linq;
using System.IO;

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
                        destinationClientContext.ExecuteQueryRetry();
                        break;
                    case FileSystemObjectType.Folder:
                        ProcessFolder(listItem, sourceWeb, destinationWeb);
                        destinationClientContext.ExecuteQueryRetry();
                        break;
                    case FileSystemObjectType.Web:
                        break;
                    default:
                        break;
                }
            }
        }

        private Microsoft.SharePoint.Client.Folder ProcessFolder(ListItem listItem
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

        private void ProcessListItem(ListItem listItem, Microsoft.SharePoint.Client.FieldCollection fields, Web sourceWeb, Web destinationWeb, List destinationList)
        {
            if (destinationList.BaseType == BaseType.DocumentLibrary)
            {

            }
            else if (destinationList.BaseType == BaseType.GenericList)
            {
                CopyItems(listItem, fields, sourceWeb, destinationWeb, destinationList);
            }
            else
            {
                // not handled other base type
            }
        }

        /// <summary>
        /// Copy files
        /// </summary>
        /// <param name="sourceLibrary"></param>
        /// <param name="destinationLibrary"></param>
        private void CopyFiles(List sourceLibrary, List destinationLibrary)
        {

        }

        /// <summary>
        /// Copy list items
        /// </summary>
        /// <param name="sourceListItem"></param>
        /// <param name="sourceFields"></param>
        /// <param name="destinationList"></param>
        private void CopyItems(ListItem sourceListItem
            , Microsoft.SharePoint.Client.FieldCollection sourceFields
            , Web sourceWeb
            , Web destinationWeb
            , List destinationList)
        {
            sourceWeb.Context.Load(sourceListItem);
            sourceWeb.Context.ExecuteQueryRetry();

            // get folder reference
            Microsoft.SharePoint.Client.Folder containerFolder = ProcessFolder(sourceListItem, sourceWeb, destinationWeb);

            // set folderUrl path to create list item inside folder.
            ListItemCreationInformation listItemCreationInformation = null;
            if (null != containerFolder)
            {
                listItemCreationInformation = new ListItemCreationInformation();
                listItemCreationInformation.FolderUrl = containerFolder.ServerRelativeUrl;
            }

            var destinationListItem = destinationList.AddItem(listItemCreationInformation);

            foreach (Microsoft.SharePoint.Client.Field field in sourceFields)
            {
                if (!field.ReadOnlyField
                    && !field.Hidden
                    && (field.InternalName != "Attachments")
                    && (field.InternalName != "ContentType"))
                {
                    try
                    {
                        destinationListItem[field.InternalName] = sourceListItem[field.InternalName];
                        destinationListItem.Update();
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            }
        }

        //private void UpdateAttachments(Web sourceWeb
        //    , Web destinationWeb
        //    , int srcItemID
        //    , int destItemID)
        //{
        //    try
        //    {
        //        string src = string.Format("{0}/lists/{1}/Attachments/{2}", sourceWeb.Url, srcItemID);
        //        Microsoft.SharePoint.Client.Folder attachmentsFolder = sourceWeb.GetFolderByServerRelativeUrl(src);
        //        sourceWeb.Context.Load(attachmentsFolder);

        //        Microsoft.SharePoint.Client.FileCollection attachments = attachmentsFolder.Files;
        //        sourceWeb.Context.Load(attachments);
        //        sourceWeb.Context.ExecuteQuery();

        //        if (attachments.Count > 0)
        //        {
        //            foreach (Microsoft.SharePoint.Client.File attachment in attachments)
        //            {
        //                ClientResult<Stream> clientResultStream = attachment.OpenBinaryStream();
        //                sourceWeb.Context.ExecuteQuery();
        //                var stream = clientResultStream.Value;

        //                AttachmentCreationInformation attachFileInfo = new AttachmentCreationInformation();
        //                Byte[] buffer = new Byte[attachment.Length];
        //                int bytesRead = stream.Read(buffer, 0, buffer.Length);
        //                MemoryStream stream2 = new MemoryStream(buffer);
        //                attachFileInfo.ContentStream = stream2;
        //                attachFileInfo.FileName = attachment.Name;
        //                //dstcontext.ExecuteQuery();

        //                //Attachment a = destitem.AttachmentFiles.Add(attachFileInfo);
        //                //dstcontext.Load(a);
        //                //dstcontext.ExecuteQuery();
        //                //stream2.Close();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Log exception
        //    }
        //}
    }
}