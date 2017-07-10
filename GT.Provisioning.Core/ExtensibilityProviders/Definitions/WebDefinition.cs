using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.ExtensibilityProviders.Definitions
{
    public class WebDefinition
    {
        public String Title { get; set; }

        public String Description { get; set; }

        public String Url { get; set; }

        public Int32 Language { get; set; }

        public Boolean UseSamePermissionsAsParentSite { get; set; }

        public Boolean InheritNavigation { get; set; }

        public String BaseTemplate { get; set; }

        public String PnPTemplate { get; set; }
    }
}
