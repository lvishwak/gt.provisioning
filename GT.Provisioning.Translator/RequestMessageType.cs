using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Translator
{
    public enum RequestMessageType
    {
        CreateTaxSymphonyPost = 0,
        CreateTaxSymphonyPre,
        SetSiteGroupMembers,
        SetSiteName,
        SetSiteStatus
    }
}
