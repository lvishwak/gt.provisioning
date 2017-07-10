using GT.Provisioning.Core.ExtensibilityProviders.Definitions;
using GT.Provisioning.Core.Jobs;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.TokenDefinitions;
using System;
using System.Collections.Generic;

namespace GT.Provisioning.Core.ExtensibilityProviders
{
    public class WebExtensibilityProvider : IProvisioningExtensibilityHandler
    {
        private readonly string logSource = "GT.Provisioning.Core.ExtensibilityProviders.WebExtensibilityProvider";

        public ProvisioningTemplate Extract(
            ClientContext ctx,
            ProvisioningTemplate template,
            ProvisioningTemplateCreationInformation creationInformation,
            PnPMonitoredScope scope,
            string configurationData)
        {
            return template;
        }

        public IEnumerable<TokenDefinition> GetTokens(
            ClientContext ctx,
            ProvisioningTemplate template,
            string configurationData)
        {
            return new List<TokenDefinition>();
        }

        public void Provision(
            ClientContext ctx,
            ProvisioningTemplate template,
            ProvisioningTemplateApplyingInformation applyingInformation,
            TokenParser tokenParser,
            PnPMonitoredScope scope,
            string configurationData)
        {
            Log.Info(logSource, $"(ProcessRequest. Template: {template.Id}. Config: {configurationData}");

            if (string.IsNullOrEmpty(configurationData))
            {
                return;
            }

            var parentWeb = ctx.Web;
            ctx.Load(parentWeb, w => w.Url);
            ctx.ExecuteQueryRetry();

            var webInformation = new XmlFormatter().ToEntity(configurationData);

            if (webInformation != null && webInformation.Webs.Count > 0)
            {
                ProvisionSubSite(parentWeb, webInformation.Webs);
            }
        }

        private void ProvisionSubSite(Web parentWeb, IEnumerable<WebDefinition> webCollection)
        {
            var parentWebUrl = parentWeb.Url;

            foreach (var web in webCollection)
            {
                ProvisioningFactory.Current.Provision(new SubSiteProvisioningJob()
                {
                    Title = web.Title,
                    Description = web.Description,
                    RelativeUrl = web.Url,
                    InheritPermissions = web.UseSamePermissionsAsParentSite,
                    InheritNavigation = web.InheritNavigation,
                    BaseTemplate = web.BaseTemplate,
                    PnPTemplate = web.PnPTemplate,
                    Language = Convert.ToUInt32(web.Language),
                    ParentWebUrl = parentWebUrl
                });
            }
        }
    }
}