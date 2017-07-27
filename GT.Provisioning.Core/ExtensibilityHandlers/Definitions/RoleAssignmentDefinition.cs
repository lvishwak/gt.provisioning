using GT.Provisioning.Core.ExtensibilityHandlers.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.ExtensibilityProviders.Definitions
{
    public class RoleAssignmentDefinition : BaseDefinition
    {
        public string Principal { get; set; }

        public string RoleDefinition { get; set; }
    }
}
