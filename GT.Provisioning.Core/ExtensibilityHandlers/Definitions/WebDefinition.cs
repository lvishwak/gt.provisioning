using GT.Provisioning.Core.ExtensibilityHandlers.Definitions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GT.Provisioning.Core.Constants;

namespace GT.Provisioning.Core.ExtensibilityProviders.Definitions
{
    public class WebDefinition : BaseDefinition
    {
        public String Title { get; set; }

        public String Description { get; set; }

        public String Url { get; set; }

        public Int32 Language { get; set; } = 1033;

        public Boolean UseSamePermissionsAsParentSite { get; set; }

        public Boolean InheritNavigation { get; set; }

        public String BaseTemplate { get; set; } = ConfigurationManager.AppSettings[AppSettings.BaseSiteTemplate_AppSetting_Key];

        public String PnPTemplate { get; set; }

        public List<RoleAssignmentDefinition> RoleAssignments { get; set; } = new List<RoleAssignmentDefinition>();

        public List<ListDefinition> ListInstances { get; set; } = new List<ListDefinition>();

        public List<PropertyDefinition> Properties { get; set; }
    }
}
