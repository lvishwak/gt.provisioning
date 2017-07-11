using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.ExtensibilityProviders.Definitions
{
    public class RoleAssignmentDefinition
    {
        public string Principal { get; set; }

        public string RoleDefinition { get; set; }
    }
}
