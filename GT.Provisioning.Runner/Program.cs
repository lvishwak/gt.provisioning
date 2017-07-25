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
            if (CommandLineArguments.ProcessCommandLineArguments(args))
            {
                string messageFilePath = CommandLineArguments.ConfigurationFilePath;

                PnPXmlTransformer transformer = new PnPXmlTransformer();
                var request = transformer.TransformXmlStringWithXslString(messageFilePath);

                ProvisioningFactory.Current.ApplyTemplate(new ApplyTemplateProvisioningJob()
                {
                    PnPTemplate = request.Request
                });
            }

            Console.WriteLine("Press any key continue....");
            Console.ReadLine();
        }
    }
}