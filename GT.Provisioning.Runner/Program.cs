using GT.Provisioning.Core;
using GT.Provisioning.Core.Jobs;
using GT.Provisioning.Translator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            PnPXmlTransformer transformer = new PnPXmlTransformer();
            var request = transformer.TransformXmlStringWithXslString("SetSiteGroupMembers.xml");

            ProvisioningFactory.Current.ApplyTemplate(new ApplyTemplateProvisioningJob()
            {
                TargetSiteUrl = "https://dreamsonline.sharepoint.com/sites/nsc/teamspace/",
                PnPTemplate = request.Request
            });

            Console.WriteLine("Press any key continue....");
            Console.ReadLine();
        }
    }
}