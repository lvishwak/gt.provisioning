using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Configuration
{
    interface IAppSettingConfiguration
    {
        string ClientId { get; }

        string ClientSecret { get; }

        string HostSiteUrl { get; }

        string TenantAdminUrl { get; }

        string PrimarySiteCollectionAdministrator { get; }

        string BaseSiteTemplate { get; }
    }
}
