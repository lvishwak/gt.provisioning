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
        public ProvisioningTemplate Extract(ClientContext ctx, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInformation, PnPMonitoredScope scope, string configurationData)
        {
            return template;
        }

        public IEnumerable<TokenDefinition> GetTokens(ClientContext ctx, ProvisioningTemplate template, string configurationData)
        {
            return new List<TokenDefinition>();
        }

        public void Provision(ClientContext ctx, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation, TokenParser tokenParser, PnPMonitoredScope scope, string configurationData)
        {
            if (string.IsNullOrEmpty(configurationData))
            {
                return;
            }

            var web = ctx.Web;
            ctx.Load(web);
            ctx.ExecuteQueryRetry();

            var configurationXmlElement = ParseXml(configurationData);
            var migrateLists = from l in configurationXmlElement.Descendants("List")
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

                // copy specified library to destination web
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
            , string sourceListUrl
            , string sourceWebUrl
            , string destinationWebUrl)
        {
            var siteCollectionContext = clientContext.GetSiteCollectionContext();
            var sourceWeb = siteCollectionContext.Site.OpenWeb(sourceWebUrl);
            var destinationWeb = siteCollectionContext.Site.OpenWeb(destinationWebUrl);

            // Get source list
            var sourceClientContext = sourceWeb.Context;
            var sourceList = sourceWeb.GetListByUrl(sourceListUrl);
            sourceClientContext.Load(sourceList);
            sourceClientContext.ExecuteQuery();

            var sourceListItems = sourceList.GetItems(CamlQuery.CreateAllItemsQuery());
            sourceClientContext.Load(sourceListItems);
            sourceClientContext.ExecuteQueryRetry();

            // update destination list
            var destinationClientContext = destinationWeb.Context;

            foreach (var listItem in sourceListItems)
            {
                switch (listItem.FileSystemObjectType)
                {
                    case FileSystemObjectType.Invalid:
                        break;
                    case FileSystemObjectType.File:
                        break;
                    case FileSystemObjectType.Folder:
                        break;
                    case FileSystemObjectType.Web:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}