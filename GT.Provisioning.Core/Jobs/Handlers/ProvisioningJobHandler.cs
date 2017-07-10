using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Jobs.Handlers
{
    internal abstract class ProvisioningJobHandler
    {
        abstract internal void RunJob(ProvisioningJob provisioningJob);        
    }
}
