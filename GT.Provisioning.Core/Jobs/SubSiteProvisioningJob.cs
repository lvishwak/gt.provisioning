using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Jobs
{
    public class SubSiteProvisioningJob : SiteProvisioningJob
    {
        /// <summary>
        /// Defines the URL of the Parent Web
        /// </summary>
        public String ParentWebUrl { get; set; }

        /// <summary>
        /// Declares whether to inherit permissions from the parent Site Collection during Site provisioning
        /// </summary>
        public Boolean InheritPermissions { get; set; } = true;

        /// <summary>
        /// Declares whether to inherit navigation from the parent Site Collection during Site provisioning
        /// </summary>
        public Boolean InheritNavigation { get; set; } = true;
    }
}
