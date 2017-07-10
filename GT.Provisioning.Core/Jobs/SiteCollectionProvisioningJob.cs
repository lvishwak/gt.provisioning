using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Jobs
{
    public class SiteCollectionProvisioningJob : SiteProvisioningJob
    {
        /// <summary>
        /// Defines the Primary Site Collection Administrator for the Site Collection to provision
        /// </summary>
        public string SiteCollectionAdministrator { get; set; }

        /// <summary>
        /// Defines the Storage Maximum Level for the Site Collection to provision
        /// </summary>
        public Int64 StorageMaximumLevel { get; set; }

        /// <summary>
        /// Defines the Storage Warning Level for the Site Collection to provision
        /// </summary>
        public Int64 StorageWarningLevel { get; set; }
    }
}
