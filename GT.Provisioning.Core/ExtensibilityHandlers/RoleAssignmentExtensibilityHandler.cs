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
using GT.Provisioning.Core.ExtensibilityProviders;
using System.Xml.Linq;
using System.IO;

namespace GT.Provisioning.Core.ExtensibilityProviders
{
    public class RoleAssignmentExtensibilityHandler : IProvisioningExtensibilityHandler
    {
        private readonly string logSource = "GT.Provisioning.Core.ExtensibilityProviders.RoleAssignmentExtensibilityHandler";

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

            var web = ctx.Web;
            ctx.Load(web
                , w => w.Url
                , w => w.SiteGroups
                , w => w.RoleAssignments
                , w => w.RoleDefinitions);

            ctx.ExecuteQueryRetry();

            ManageGroupAssignment(web, configurationData, scope);
        }

        private void ManageGroupAssignment(Web web, string groupAssignmentConfigurationXml, PnPMonitoredScope Log)
        {
            // parse xml
            var groupAssignmentXmlElement = ParseXml(groupAssignmentConfigurationXml);
            var groupAssignments = from g in groupAssignmentXmlElement.Descendants("GroupAssignment")
                                   select new
                                   {
                                       name = g.Attribute("group").Value,
                                       users = g.Descendants("User").Any() ? (from u in g.Descendants("User")
                                                                              select new
                                                                              {
                                                                                  userLoginName = u.Attribute("loginName").Value,
                                                                                  action = u.Attribute("action").Value,
                                                                              }).ToList()
                                                                              : null
                                   };

            foreach (var groupAssignment in groupAssignments)
            {
                var groupName = groupAssignment.name;
                if (!web.GroupExists(groupName))
                {
                    Log.LogInfo($"Group {groupName} does not exsits.");
                    continue;
                }

                foreach (var user in groupAssignment.users)
                {
                    if (user.action.ToLowerInvariant() == "add")
                    {
                        if (web.IsUserInGroup(groupName, user.userLoginName))
                        {
                            Log.LogInfo($"User {user.userLoginName} already exists in group {groupName}.");
                            continue;
                        }

                        Log.LogInfo($"Adding user {user.userLoginName} to group {groupName}.");
                        web.AddUserToGroup(groupName, user.userLoginName);
                    }

                    if (user.action.ToLowerInvariant() == "remove")
                    {
                        if (web.IsUserInGroup(groupName, user.userLoginName))
                        {
                            var siteUser = web.EnsureUser(user.userLoginName);
                            var siteGroup = web.SiteGroups.GetByName(groupName);
                            web.Context.Load(siteGroup);
                            web.Context.ExecuteQueryRetry();
                            if (siteGroup != null)
                            {
                                Log.LogInfo($"Removing user {user.userLoginName} from group {groupName}.");
                                web.RemoveUserFromGroup(siteGroup, siteUser);
                            }                                
                        }

                        Log.LogInfo($"User {user.userLoginName} does not exists in group {groupName}.");
                    }
                }
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
    }
}