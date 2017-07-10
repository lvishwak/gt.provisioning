using GT.Provisioning.Core;
using GT.Provisioning.Core.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            ProvisioningFactory.Current.Provision(
                new SiteCollectionProvisioningJob()
                {
                    Title = "New Site Collection",
                    Description = "",
                    RelativeUrl = "nsc",
                    PnPTemplate = "gt-clienthub-template.xml",
                    TimeZone = 11,
                    Language = 1033
                });

            Console.ReadLine();
        }
    }
}