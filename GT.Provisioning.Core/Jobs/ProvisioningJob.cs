using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Jobs
{
    public class ProvisioningJob
    {
        /// <summary>
        /// The ID of the Provisioning Job
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// Defines the Status of the Provisioning Job
        /// </summary>
        public JobStatus Status { get; set; }

        /// <summary>
        /// Defines the type of Provisioning Job (SiteCollection, Subsite or other type)
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// Defines the Error Message of the Provisioning Job, if any
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Defines the creation time stamp of the provisioning job.
        /// </summary>
        public DateTime CreationTimeStamp { get; set; }

        /// <summary>
        /// Defines the user who has created the provisioning job.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Defines the modified time stamp of the provisioning job.
        /// </summary>
        public DateTime ModifiedTimeStamp { get; set; }

        /// <summary>
        /// Defines the user who has modified the provisioning job.
        /// </summary>
        public string ModifiedBy { get; set; }
    }
}
