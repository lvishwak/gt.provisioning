using GT.Provisioning.Core.ExtensibilityProviders.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.ExtensibilityProviders
{
    public class WebInformation
    {
        public List<WebDefinition> Webs { get; set; } = new List<WebDefinition>();
    }
}
