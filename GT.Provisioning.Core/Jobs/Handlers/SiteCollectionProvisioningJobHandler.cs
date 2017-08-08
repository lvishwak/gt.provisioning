using GT.Provisioning.Core.Authentication;
using GT.Provisioning.Core.Configuration;
using GT.Provisioning.Core.Repositories;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using System;
using System.Threading;

namespace GT.Provisioning.Core.Jobs.Handlers
{
    internal class SiteCollectionProvisioningJobHandler : ProvisioningJobHandler
    {
        internal override void RunJob(ProvisioningJob provisioningJob)
        {
            var siteCollectionProvisioningJob = provisioningJob as SiteCollectionProvisioningJob;
            if (siteCollectionProvisioningJob == null)
            {
                throw new ArgumentException("$(Invalid job type for SiteCollectionPrivisioningJobHandler)");
            }

            CreateSiteCollection(siteCollectionProvisioningJob);
        }

        private void CreateSiteCollection(SiteCollectionProvisioningJob job)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("CreateSiteCollection"))
            {
                // build site collection url
                String siteUrl = String.Format("{0}{1}",
                    ConfigurationHelper.GetConfiguration.HostSiteUrl.Substring(0, ConfigurationHelper.GetConfiguration.HostSiteUrl.LastIndexOf("/") + 1),
                    job.RelativeUrl.TrimStart('/'));

                // get provisioning template
                var provisioningTemplate = GetProvisioningTemplate(job.PnPTemplate);

                if (provisioningTemplate != null)
                {
                    job.BaseTemplate = string.IsNullOrEmpty(provisioningTemplate.BaseSiteTemplate)
                        ? (string.IsNullOrEmpty(job.BaseTemplate)
                            ? ConfigurationHelper.GetConfiguration.BaseSiteTemplate
                            : job.BaseTemplate)
                        : provisioningTemplate.BaseSiteTemplate;
                }
                else
                {
                    job.BaseTemplate = string.IsNullOrEmpty(job.BaseTemplate)
                            ? ConfigurationHelper.GetConfiguration.BaseSiteTemplate
                            : job.BaseTemplate;
                }

                using (var adminContext = AppOnlyContextProvider.GetAppOnlyTenantLevelClientContext())
                {
                    adminContext.RequestTimeout = Timeout.Infinite;

                    // Create the Site Collection and wait for its creation (we're asynchronous)
                    var tenant = new Tenant(adminContext);

                    // check if site collection already exists and in active state.
                    if (tenant.CheckIfSiteExists(siteUrl, Constants.Site_Status_Active))
                    {
                        Log.LogError($"Site collection with url \"{siteUrl}\" already exists.");
                    }
                    else
                    {
                        Log.LogInfo($"Creating site collection \"{job.RelativeUrl}\" with template {job.BaseTemplate})");

                        try
                        {
                            tenant.CreateSiteCollection(new SiteEntity()
                            {
                                Description = job.Description,
                                Title = job.Title,
                                Url = siteUrl,
                                SiteOwnerLogin = ConfigurationHelper.GetConfiguration.PrimarySiteCollectionAdministrator,
                                StorageMaximumLevel = job.StorageMaximumLevel,
                                StorageWarningLevel = job.StorageWarningLevel,
                                Template = job.BaseTemplate,
                                Lcid = ((provisioningTemplate != null && provisioningTemplate.RegionalSettings != null)
                                        && provisioningTemplate.RegionalSettings.LocaleId > 0)
                                        ? uint.Parse(provisioningTemplate.RegionalSettings.LocaleId.ToString())
                                        : job.Language,
                                TimeZoneId = ((provisioningTemplate != null && provisioningTemplate.RegionalSettings != null)
                                        && provisioningTemplate.RegionalSettings.TimeZone > 0)
                                        ? provisioningTemplate.RegionalSettings.TimeZone
                                        : job.TimeZone,
                            }, removeFromRecycleBin: true, wait: true);
                        }
                        catch (Exception exception)
                        {
                            Log.LogError($"Error occured while creating site collection {job.RelativeUrl}. Inner exception: {exception.Message}");

                            if (tenant.SiteExists(siteUrl))
                            {
                                tenant.DeleteSiteCollection(siteUrl, useRecycleBin: false);
                            }

                            throw exception;
                        }
                    }

                    if (provisioningTemplate != null)
                    {
                        Log.LogInfo($"Applying provisioning template {provisioningTemplate.DisplayName}");
                        ApplyProvisioningTemplate(provisioningTemplate, siteUrl);
                    }

                    Log.LogInfo($"Site collection {siteUrl} provisioned successfully.");
                }
            }
        }

        private ProvisioningTemplate GetProvisioningTemplate(string uri)
        {
            var templateRepository = new TemplateRepository();
            return templateRepository.GetTemplate(uri);
        }

        private void ApplyProvisioningTemplate(ProvisioningTemplate template, string siteCollectionUrl)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("ApplyProvisioningTemplate"))
            {
                using (var siteContext = AppOnlyContextProvider.GetSharePointOnlineAuthenticatedContext(siteCollectionUrl))
                {
                    Site site = siteContext.Site;
                    Web web = site.RootWeb;

                    siteContext.Load(site, s => s.Url);
                    siteContext.Load(web, w => w.Url);
                    siteContext.ExecuteQueryRetry();

                    try
                    {
                        var applyingInfo = new ProvisioningTemplateApplyingInformation
                        {
                            ProgressDelegate =
                                (message, step, total) =>
                                {
                                    Log.LogInfo($"{step}/{total} Provisioning {message}");
                                }
                        };

                        web.ApplyProvisioningTemplate(template, applyingInfo);
                    }
                    catch (Exception exception)
                    {
                        Log.LogError($"Error occured while applying template {template.DisplayName}: {exception.Message}");
                    }
                }
            }
        }
    }
}