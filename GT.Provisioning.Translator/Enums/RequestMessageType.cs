using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Translator
{
    public enum RequestMessageType
    {
        None = 0,
        TaxSymphonyPost,
        TaxSymphonyPre,
        SetSiteGroupMembers,
        SetSiteName,
        SetSiteStatus
    }
}
