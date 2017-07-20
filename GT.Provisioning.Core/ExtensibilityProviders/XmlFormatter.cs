using GT.Provisioning.Core.ExtensibilityProviders.Definitions;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

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
                        BaseTemplate = w.BaseTemplate,
                        PnPTemplate = w.PnPTemplate,
                        Language = w.Language,
                        RoleAssignments = w.RoleAssignments.Any() ? (from roleAssignment in w.RoleAssignments
                                                                     select new RoleAssignmentDefinition
                                                                     {
                                                                         Principal = roleAssignment.Principal,
                                                                         RoleDefinition = roleAssignment.RoleDefinition
                                                                     }).ToList()
                                         : null
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