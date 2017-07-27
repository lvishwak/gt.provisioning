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
                    job.TargetSiteUrl = provisioningTemplate.Parameters["siteid"];
                }

                var siteCollectionUrl = job.TargetSiteUrl;

                using (var adminContext = AppOnlyContextProvider.GetAppOnlyTenantLevelClientContext())
                {
                    adminContext.RequestTimeout = Timeout.Infinite;

                    // Create the Site Collection and wait for its creation (we're asynchronous)
                    var tenant = new Tenant(adminContext);

                    // check if site already exists.
                    if (tenant.CheckIfSiteExists(siteCollectionUrl, Constants.Site_Status_Active))
                    {
                        Log.LogInfo($"Site with url \"{siteCollectionUrl}\" already exists. Applying template.");
                        ApplyProvisioningTemplate(siteCollectionUrl, provisioningTemplate, Log);
                    }
                    else
                    {
                        // check for site collection or subsite
                        if (IsSiteCollection(siteCollectionUrl))
                        {
                            try
                            {
                                tenant.CreateSiteCollection(new SiteEntity()
                                {
                                    Title = provisioningTemplate.Parameters["sitetitle"],
                                    Url = siteCollectionUrl,
                                    SiteOwnerLogin = ConfigurationHelper.GetConfiguration.PrimarySiteCollectionAdministrator,
                                    StorageMaximumLevel = 100,
                                    StorageWarningLevel = 70,
                                    Template = ConfigurationHelper.GetConfiguration.BaseSiteTemplate,
                                    Lcid = 1033,
                                    TimeZoneId = 13,
                                }, removeFromRecycleBin: true, wait: true);

                                ApplyProvisioningTemplate(siteCollectionUrl, provisioningTemplate, Log);
                            }
                            catch (Exception exception)
                            {
                                Log.LogError($"Error occured while creating site collection {siteCollectionUrl}. Inner exception: {exception.Message}");

                                if (tenant.SiteExists(siteCollectionUrl))
                                {
                                    tenant.DeleteSiteCollection(siteCollectionUrl, useRecycleBin: false);
                                }

                                throw exception;
                            }
                        }

                        Log.LogInfo($"Successfully applied template applied to site with url {siteCollectionUrl}.");
                    }
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
            using (var appOnlyClientContext = AppOnlyContextProvider.GetAppOnlyClientContext(siteUrl))
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