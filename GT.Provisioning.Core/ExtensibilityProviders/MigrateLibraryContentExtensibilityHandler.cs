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

            string listTitle = "Review Notes";
            string destinationWebRelativeUrl = "/sites/00123/clientspace";

            var web = ctx.Web;
            ctx.Load(web);
            ctx.ExecuteQueryRetry();

            // copy specified library to destination web
            MigrateListItems(web, listTitle, destinationWebRelativeUrl);
        }

        private void MigrateListItems(Web sourceWeb, string sourceListTitle, string destinationSiteUrl)
        {
            // Get source list
            var sourceClientContext = sourceWeb.Context;
            var lists = sourceClientContext.LoadQuery(sourceWeb.Lists.Where(l => l.Title.ToUpperInvariant() == sourceListTitle.ToUpperInvariant()));
            sourceClientContext.ExecuteQuery();

            var sourceList = lists.FirstOrDefault();
            if (null == sourceList)
            {
                throw new ArgumentNullException(nameof(sourceList));
            }

            // get all items to migrate including folder
            var sourceListItems = sourceList.GetItems(CamlQuery.CreateAllItemsQuery());
            sourceClientContext.Load(sourceListItems);
            sourceClientContext.ExecuteQueryRetry();

            var siteCollectionContext = sourceWeb.Context.GetSiteCollectionContext();
            var destinationWeb = siteCollectionContext.Site.OpenWeb(destinationSiteUrl);

            var destinationClientContext = destinationWeb.Context;

            // copy items
            foreach (var listItem in sourceListItems)
            {
                
            }
        }
    }
}
