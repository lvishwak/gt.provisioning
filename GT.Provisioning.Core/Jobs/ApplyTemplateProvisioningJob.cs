using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Jobs
{
    public class ApplyTemplateProvisioningJob : ProvisioningJob
    {
        public string TargetSiteUrl { get; set; }

        public ProvisioningTemplate PnPTemplate { get; set; }
    }
}
