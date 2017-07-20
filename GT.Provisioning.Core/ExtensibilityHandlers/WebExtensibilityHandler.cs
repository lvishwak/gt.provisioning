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

                // if unique permissions defined
                if (web.RoleAssignments.Any())
                {
                    if (parentWeb.WebExists(web.Url))
                    {
                        Web newWeb = parentWeb.GetWeb(web.Url);

                        // create unique role assignments for web
                        newWeb.BreakRoleInheritance(copyRoleAssignments: false, clearSubscopes: false);

                        foreach (var roleAssignment in web.RoleAssignments)
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
}