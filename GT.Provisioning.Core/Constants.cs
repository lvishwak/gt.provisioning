using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core
{
    internal class Constants
    {
        internal const string Site_Status_Active = "Active";

        internal class AppSettings
        {
            internal const string ClientId_AppSetting_Key = "ClientId";
            internal const string ClientSecret_AppSetting_Key = "ClientSecret";
            internal const string HostSiteUrl_AppSetting_Key = "HostSiteUrl";
            internal const string TenantAdminUrl_AppSetting_Key = "TenantAdminUrl";
            internal const string PrimarySiteCollectionAdmin_AppSetting_Key = "PrimarySiteCollectionAdmin";
            internal const string BaseSiteTemplate_AppSetting_Key = "BaseSiteTemplate";
        }

        internal class SiteProperties
        {
            internal const Int64 StorageMaximumLevel = 100;
            internal const Int64 StorageWarningLevel = 70;
            internal const Int32 Lcid = 1033;
            internal const Int16 CentralTimeZone = 13; 
        }
    }
}
