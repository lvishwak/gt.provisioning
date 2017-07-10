using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core
{
    public enum JobStatus
    {
        /// <summary>
        /// The Provisioning Job is still pending
        /// </summary>
        Pending = 2,
        /// <summary>
        /// The Provisioning Job is has been queued and awaiting for processing
        /// </summary>
        Queued = 4,
        /// <summary>
        /// The Provisioning Job is running
        /// </summary>
        Running = 8,
        /// <summary>
        /// The Provisioning Job has been cancelled
        /// </summary>
        Cancelled = 16,
        /// <summary>
        /// The Provisioning Job failed
        /// </summary>
        Failed = 32,
        /// <summary>
        /// The Provisioning Job as been completed
        /// </summary>
        Provisioned = 64
    }
}
