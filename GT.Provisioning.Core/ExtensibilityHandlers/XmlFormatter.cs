using GT.Provisioning.Core.ExtensibilityHandlers.Definitions;
using GT.Provisioning.Core.ExtensibilityProviders.Definitions;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using static GT.Provisioning.Core.Constants;

namespace GT.Provisioning.Core.ExtensibilityProviders
{
    internal class XmlFormatter
    {
        public WebInformation ToEntity(string xmlConfiguration)
        {
            if (string.IsNullOrEmpty(xmlConfiguration))
            {
                throw new ArgumentNullException(nameof(xmlConfiguration));
            }

            byte[] encodedString = Encoding.UTF8.GetBytes(xmlConfiguration);
            MemoryStream sourceStream = new MemoryStream(encodedString);
            sourceStream.Flush();
            sourceStream.Position = 0;

            XDocument xml = XDocument.Load(sourceStream);

            // This is required to remove namespace attribute else XmlSerializer throws exception
            var updatedXmlConfiguration = RemoveXmlns(xml);

            WebInformation source = new WebInformation();

            // Deserialize template
            var result = XMLSerializer.Deserialize<ExtensibilityHandlers.XML.WebProviderConfiguration>(updatedXmlConfiguration.InnerXml);

            if (result != null)
            {
                if (result.webs != null && result.webs.Any())
                {
                    source.Webs.AddRange(result.webs.Select(w => new WebDefinition()
                    {
                        Title = w.Title,
                        Description = w.Description,
                        Url = w.Url,
                        UseSamePermissionsAsParentSite = w.UseSamePermissionsAsParentSite,
                        InheritNavigation = w.InheritNavigation,
                        BaseTemplate = (string.IsNullOrEmpty(w.BaseTemplate)) ? ConfigurationManager.AppSettings[AppSettings.BaseSiteTemplate_AppSetting_Key] : w.BaseTemplate,
                        PnPTemplate = w.PnPTemplate,
                        Language = (w.Language == default(int)) ? 1033 : w.Language,
                        RoleAssignments = (w.RoleAssignments != null && w.RoleAssignments.Any())
                        ? (from roleAssignment in w.RoleAssignments
                           select new RoleAssignmentDefinition
                           {
                               Principal = roleAssignment.Principal,
                               RoleDefinition = roleAssignment.RoleDefinition
                           }).ToList() : null,
                        ListInstances = (w.Lists != null && w.Lists.Any())
                        ? (from list in w.Lists
                           select new ListDefinition
                           {
                               Title = list.name,
                               Url = list.url,
                               TemplateId = list.templateType,
                               Folders = (list.Folders != null && list.Folders.Any())
                               ? (from f in list.Folders
                                  select new FolderDefinition
                                  {
                                      Name = f.name
                                  }).ToList() : null
                           }).ToList() : null
                    }));
                }
            }

            return source;
        }

        public XmlDocument RemoveXmlns(XDocument d)
        {
            d.Root.DescendantsAndSelf().Attributes().Where(x => x.IsNamespaceDeclaration).Remove();

            foreach (var elem in d.Descendants())
                elem.Name = elem.Name.LocalName;

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(d.CreateReader());

            return xmlDocument;
        }
    }
}