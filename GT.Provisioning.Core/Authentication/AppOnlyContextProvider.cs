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
        internal static ClientContext GetAppOnlyContext(string siteUrl, string appId = null, string appSecret = null)
        {
            if (string.IsNullOrWhiteSpace(siteUrl))
            {
                throw new ArgumentNullException(nameof(siteUrl));
            }

            appId = appId ?? Configuration.ConfigurationHelper.GetConfiguration.ClientId;
            appSecret = appSecret ?? Configuration.ConfigurationHelper.GetConfiguration.ClientSecret;

            AuthenticationManager authenticationManager = new AuthenticationManager();
            var clientContext = authenticationManager.GetAppOnlyAuthenticatedContext(siteUrl, appId, appSecret);

            return clientContext;
        }

        internal static ClientContext GetSharePointOnlineAuthenticatedContext(string siteUrl, string tenantUser, string tenantPassword)
        {
            if (string.IsNullOrWhiteSpace(siteUrl))
            {
                throw new ArgumentNullException(nameof(siteUrl));
            }

            AuthenticationManager authenticationManager = new AuthenticationManager();
            var clientContext = authenticationManager.GetSharePointOnlineAuthenticatedContextTenant(siteUrl, tenantUser, tenantPassword);

            return clientContext;
        }
    }
}