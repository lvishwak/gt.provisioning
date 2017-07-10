using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Repositories
{
    class TemplateRepository : ITemplateRepository
    {
        public ProvisioningTemplate GetTemplate(string templateUri)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("GetTemplate"))
            {
                try
                {
                    var provider = new XMLFileSystemTemplateProvider(AppDomain.CurrentDomain.BaseDirectory, "");
                    return provider.GetTemplate(templateUri);
                }
                catch (Exception exception)
                {
                    Log.LogError(exception.Message);                    
                }

                return null;
            }            
        }
    }
}
