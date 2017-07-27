using GT.Provisioning.Core.ExtensibilityProviders.Definitions;
using GT.Provisioning.Core.Jobs;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.TokenDefinitions;
using System;
using System.Linq;
using System.Collections.Generic;
using GT.Provisioning.Core.ExtensibilityHandlers.ModelHandler;

namespace GT.Provisioning.Core.ExtensibilityProviders
{
    public class WebExtensibilityHandler : IProvisioningExtensibilityHandler
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
            ctx.Load(parentWeb
                , w => w.Url
                , w => w.SiteGroups
                , w => w.RoleAssignments
                , w => w.RoleDefinitions);

            ctx.ExecuteQueryRetry();

            var webInformation = new XmlFormatter().ToEntity(configurationData);
            if (webInformation != null && webInformation.Webs.Count > 0)
            {
                ProvisionSubSite(parentWeb, webInformation.Webs);
            }
        }

        private void ProvisionSubSite(Web parentWeb, IEnumerable<WebDefinition> webDefinitions)
        {
            var parentWebUrl = parentWeb.Url;

            foreach (var webDefinition in webDefinitions)
            {
                ProvisioningFactory.Current.Provision(new SubSiteProvisioningJob()
                {
                    Title = webDefinition.Title,
                    Description = webDefinition.Description,
                    RelativeUrl = webDefinition.Url,
                    InheritPermissions = webDefinition.UseSamePermissionsAsParentSite,
                    InheritNavigation = webDefinition.InheritNavigation,
                    BaseTemplate = webDefinition.BaseTemplate,
                    PnPTemplate = webDefinition.PnPTemplate,
                    Language = Convert.ToUInt32(webDefinition.Language),
                    ParentWebUrl = parentWebUrl
                });

                Web newWeb = parentWeb.GetWeb(webDefinition.Url);
                if (webDefinition.ListInstances.Count > 0)
                {
                    ListModelHandler handler = new ListModelHandler();
                    handler.Deploy(newWeb, webDefinition);
                }

                // if unique permissions defined
                if (webDefinition.RoleAssignments != null && webDefinition.RoleAssignments.Any())
                {
                    // create unique role assignments for web
                    newWeb.BreakRoleInheritance(copyRoleAssignments: false, clearSubscopes: false);

                    foreach (var roleAssignment in webDefinition.RoleAssignments)
                    {
                        var siteGroup
                            = parentWeb.SiteGroups
                                .Where(
                                    g => g.Title == roleAssignment.Principal).FirstOrDefault();

                        var permissionLevel
                            = parentWeb.RoleDefinitions
                                .Where(
                                    r => r.Name == roleAssignment.RoleDefinition).FirstOrDefault();

                        if (siteGroup != null && permissionLevel != null)
                        {
                            var roleDefinitionBindingCollection = new RoleDefinitionBindingCollection(parentWeb.Context)
                                {
                                    permissionLevel
                                };

                            newWeb.RoleAssignments.Add(siteGroup, roleDefinitionBindingCollection);
                        }
                    }
                }
            }
        }
    }
}