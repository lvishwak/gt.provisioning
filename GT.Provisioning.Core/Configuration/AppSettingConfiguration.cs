using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Configuration
{
    internal class AppSettingConfiguration : IAppSettingConfiguration
    {
        public string ClientId => ConfigurationManager.AppSettings[Constants.AppSettings.ClientId_AppSetting_Key];

        public string ClientSecret => ConfigurationManager.AppSettings[Constants.AppSettings.ClientSecret_AppSetting_Key];

        public string HostSiteUrl => ConfigurationManager.AppSettings[Constants.AppSettings.HostSiteUrl_AppSetting_Key];

        public string PrimarySiteCollectionAdministrator => ConfigurationManager.AppSettings[Constants.AppSettings.PrimarySiteCollectionAdmin_AppSetting_Key];

        public string BaseSiteTemplate => ConfigurationManager.AppSettings[Constants.AppSettings.BaseSiteTemplate_AppSetting_Key];

        public string TenantAdminUser => ConfigurationManager.AppSettings[Constants.AppSettings.TenantAdmin_AppSetting_Key];

        public string TenantAdminPassword => ConfigurationManager.AppSettings[Constants.AppSettings.TenantAdminPassword_AppSetting_Key];
    }
}
