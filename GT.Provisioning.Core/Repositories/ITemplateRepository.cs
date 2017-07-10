using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Repositories
{
    interface ITemplateRepository
    {
        ProvisioningTemplate GetTemplate(string templateUri);
    }
}
