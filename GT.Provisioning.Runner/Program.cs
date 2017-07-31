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
            try
            {
                if (CommandLineArguments.ProcessCommandLineArguments(args))
                {
                    string messageFilePath = CommandLineArguments.ConfigurationFilePath;                    
                    XmlTransformer transformer = new XmlTransformer();
                    var request = transformer.TransformXmlStringWithXslString(messageFilePath);

                    ProvisioningFactory.Current.ApplyTemplate(new ApplyTemplateProvisioningJob()
                    {
                        PnPTemplate = request.Request
                    });
                }                
            }
            catch (Exception exception)
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.Message);
                Console.ForegroundColor = originalColor;
            }

            Console.WriteLine("Press any key continue....");
            Console.ReadLine();
        }
    }
}