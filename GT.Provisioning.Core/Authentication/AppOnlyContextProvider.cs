using GT.Provisioning.Core.Configuration;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Authentication
{
    internal class AppOnlyContextProvider
    {
        internal static ClientContext GetAppOnlyTenantLevelClientContext()
        {
            Uri hostSiteUri = new Uri(ConfigurationHelper.GetConfiguration.HostSiteUrl);
            Uri tenantAdminUri = new Uri(hostSiteUri.Scheme + "://" +
                hostSiteUri.Host.Replace(".sharepoint.com", "-admin.sharepoint.com"));

            return (GetSharePointOnlineAuthenticatedContext(tenantAdminUri.ToString()));
        }

        //internal static ClientContext GetAppOnlyClientContext(
        //    string siteUrl,
        //    string appId = null,
        //    string appSecret = null)
        //{
        //    if (string.IsNullOrWhiteSpace(siteUrl))
        //    {
        //        throw new ArgumentNullException(nameof(siteUrl));
        //    }

        //    AuthenticationManager authenticationManager = new AuthenticationManager();

        //    // check for client id and secret
        //    appId = appId ?? ConfigurationHelper.GetConfiguration.ClientId;
        //    appSecret = appSecret ?? ConfigurationHelper.GetConfiguration.ClientSecret;

        //    if (!string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(appSecret))
        //    {
        //        throw new ArgumentNullException(nameof(appId), $"Update configuration file with client id and secret.");
        //    }

        //    return authenticationManager.GetAppOnlyAuthenticatedContext(siteUrl, appId, appSecret);
        //}

        internal static ClientContext GetSharePointOnlineAuthenticatedContext(
            string siteUrl, 
            string tenantUser = null, 
            string tenantPassword = null)
        {
            if (string.IsNullOrWhiteSpace(siteUrl))
            {
                throw new ArgumentNullException(nameof(siteUrl));
            }

            tenantUser = tenantUser ?? ConfigurationHelper.GetConfiguration.TenantAdminUser;
            tenantPassword = tenantPassword ?? ConfigurationHelper.GetConfiguration.TenantAdminPassword;

            AuthenticationManager authenticationManager = new AuthenticationManager();
            var clientContext = authenticationManager.GetSharePointOnlineAuthenticatedContextTenant(siteUrl, tenantUser, tenantPassword);

            return clientContext;
        }
    }
}