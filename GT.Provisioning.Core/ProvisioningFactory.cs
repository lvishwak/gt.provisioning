using GT.Provisioning.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core
{
    public static class ProvisioningFactory
    {
        private static readonly Lazy<IProvisioningRepository> _provisioningRepository =
            new Lazy<IProvisioningRepository>(() =>
            {
                Type provisioningRepositoryType = Type.GetType("GT.Provisioning.Core.Repositories.ProvisioningRepository", true);
                return (IProvisioningRepository)Activator.CreateInstance(provisioningRepositoryType);
            });

        public static IProvisioningRepository Current
        {
            get
            {
                return _provisioningRepository.Value;
            }
        }
    }
}
