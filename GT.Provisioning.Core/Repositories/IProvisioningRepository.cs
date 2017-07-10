using GT.Provisioning.Core.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Repositories
{
    public interface IProvisioningRepository
    {
        void Provision(ProvisioningJob provisioningJob);
    }
}
