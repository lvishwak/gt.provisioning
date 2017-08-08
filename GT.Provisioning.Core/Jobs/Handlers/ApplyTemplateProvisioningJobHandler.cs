using GT.Provisioning.Core.Authentication;
using GT.Provisioning.Core.Configuration;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.IO;
using System.Threading;

namespace GT.Provisioning.Core.Jobs.Handlers
{
    internal class ApplyTemplateProvisioningJobHandler : ProvisioningJobHandler
    {
        internal override void RunJob(ProvisioningJob provisioningJob)
        {
            var applyTemplateProvisioningJob = provisioningJob as ApplyTemplateProvisioningJob;
            if (null == applyTemplateProvisioningJob)
            {
                throw new ArgumentException("$(Invalid job type for ApplyTemplateProvisioningJobHandler)");
            }

            ApplyTemplate(applyTemplateProvisioningJob);
        }

        private void ApplyTemplate(ApplyTemplateProvisioningJob job)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("ApplyTemplate"))
            {
                var provisioningTemplate = GetProvisioningTemplateFromStream(job.PnPTemplate);
                if (provisioningTemplate.Parameters.Count > 0
                    && provisioningTemplate.Parameters.ContainsKey("siteid"))
                {
                    var sharepointOnlineUrl = ConfigurationHelper.GetConfiguration.HostSiteUrl.Substring(0, ConfigurationHelper.GetConfiguration.HostSiteUrl.LastIndexOf("/") + 1);
                    string siteId = provisioningTemplate.Parameters["siteid"];
                    if (siteId.Contains(sharepointOnlineUrl))
                    {
                        job.TargetSiteUrl = provisioningTemplate.Parameters["siteid"];
                    }
                    else
                    {
                        // build complete url
                        job.TargetSiteUrl = String.Format("{0}{1}",
                            ConfigurationHelper.GetConfiguration.HostSiteUrl.Substring(0, ConfigurationHelper.GetConfiguration.HostSiteUrl.LastIndexOf("/") + 1),
                            provisioningTemplate.Parameters["siteid"].TrimStart('/'));
                    }
                }

                var siteFullUrl = job.TargetSiteUrl;

                // need to refactor this code to remove site collection creation code to factory
                // it should only contains code to apply template if site collection or subsite exists.
                using (var adminContext = AppOnlyContextProvider.GetAppOnlyTenantLevelClientContext())
                {
                    adminContext.RequestTimeout = Timeout.Infinite;

                    // Create the Site Collection and wait for its creation (we're asynchronous)
                    var tenant = new Tenant(adminContext);

                    // check if site already exists.
                    if (tenant.CheckIfSiteExists(siteFullUrl, Constants.Site_Status_Active))
                    {
                        Log.LogInfo($"Site with url \"{siteFullUrl}\" already exists. Applying template.");
                        ApplyProvisioningTemplate(siteFullUrl, provisioningTemplate, Log);
                    }
                    else
                    {
                        // check for site collection or subsite
                        if (IsSiteCollection(siteFullUrl))
                        {
                            try
                            {
                                Log.LogInfo($"Creating site collection \"{siteFullUrl}\"");

                                tenant.CreateSiteCollection(new SiteEntity()
                                {
                                    Title = provisioningTemplate.Parameters["sitetitle"],
                                    Url = siteFullUrl,
                                    SiteOwnerLogin = ConfigurationHelper.GetConfiguration.PrimarySiteCollectionAdministrator,
                                    StorageMaximumLevel = Constants.SiteProperties.StorageMaximumLevel,
                                    StorageWarningLevel = Constants.SiteProperties.StorageWarningLevel,
                                    Template = ConfigurationHelper.GetConfiguration.BaseSiteTemplate,
                                    Lcid = Constants.SiteProperties.Lcid,
                                    TimeZoneId = Constants.SiteProperties.CentralTimeZone
                                }, removeFromRecycleBin: true, wait: true);

                                ApplyProvisioningTemplate(siteFullUrl, provisioningTemplate, Log);
                            }
                            catch (Exception exception)
                            {
                                Log.LogError($"Error occured while creating site collection {siteFullUrl}. Inner exception: {exception.Message}");

                                if (tenant.SiteExists(siteFullUrl))
                                {
                                    tenant.DeleteSiteCollection(siteFullUrl, useRecycleBin: false);
                                }

                                throw exception;
                            }
                        }
                    }

                    Log.LogInfo($"Successfully applied template applied to site with url {siteFullUrl}.");
                }
            }
        }

        private ProvisioningTemplate GetProvisioningTemplateFromStream(Stream fileStream)
        {
            var schemaFormatter = XMLPnPSchemaFormatter.LatestFormatter;
            return schemaFormatter.ToProvisioningTemplate(fileStream);
        }

        private void ApplyProvisioningTemplate(string siteUrl, ProvisioningTemplate provisioningTemplate, PnPMonitoredScope Log)
        {
            using (var appOnlyClientContext = AppOnlyContextProvider.GetSharePointOnlineAuthenticatedContext(siteUrl))
            {
                try
                {
                    var targetWeb = appOnlyClientContext.Web;

                    var applyingInfo = new ProvisioningTemplateApplyingInformation
                    {
                        ProgressDelegate =
                            (message, step, total) =>
                            {
                                Log.LogInfo($"{step}/{total} Provisioning {message}");
                            }
                    };

                    targetWeb.ApplyProvisioningTemplate(provisioningTemplate, applyingInfo);
                }
                catch (Exception exception)
                {
                    Log.LogError(exception, $"Error occured while applying template to site {siteUrl}. Inner exception: {exception.Message}");
                }
            }
        }

        private bool IsSiteCollection(string siteFullUrl)
        {
            var url = new Uri(siteFullUrl);
            var siteDomainUrl = url.GetLeftPart(UriPartial.Authority);
            int siteNameIndex = url.AbsolutePath.IndexOf('/', 1) + 1;
            var managedPath = url.AbsolutePath.Substring(0, siteNameIndex);
            var siteRelativePath = url.AbsolutePath.Substring(siteNameIndex);

            return (siteRelativePath.IndexOf('/') == -1);
        }
    }
}