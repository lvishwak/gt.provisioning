using GT.Provisioning.Core.Authentication;
using GT.Provisioning.Core.Configuration;
using GT.Provisioning.Core.Repositories;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Jobs.Handlers
{
    internal class SubSiteProvisioningJobHandler : ProvisioningJobHandler
    {
        internal override void RunJob(ProvisioningJob provisioningJob)
        {
            var subSiteProvisioningJob = provisioningJob as SubSiteProvisioningJob;
            if (subSiteProvisioningJob == null)
            {
                throw new ArgumentException("$(Invalid job type for SubSiteProvisioningJobHandler)");
            }

            CreateSubSite(subSiteProvisioningJob);
        }

        private void CreateSubSite(SubSiteProvisioningJob job)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("CreateSubSite"))
            {
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

                Web newWeb = null;

                using (var siteContext = AppOnlyContextProvider.GetAppOnlyClientContext(job.ParentWebUrl))
                {
                    Web parentWeb = siteContext.Web;
                    siteContext.Load(parentWeb, w => w.Language, w => w.RegionalSettings.TimeZone);
                    siteContext.ExecuteQueryRetry();

                    if (parentWeb.WebExists(job.RelativeUrl))
                    {
                        Log.LogError($"Web with url \"{job.RelativeUrl}\" already exists.");
                        newWeb = parentWeb.GetWeb(job.RelativeUrl);
                        siteContext.Load(newWeb);
                        siteContext.ExecuteQueryRetry();
                    }
                    else
                    {
                        Log.LogInfo($"Creating web \"{job.RelativeUrl}\" with template {job.BaseTemplate}");

                        try
                        {
                            // Create the new sub site as a new child Web
                            newWeb = parentWeb.CreateWeb(new SiteEntity()
                            {
                                Title = job.Title,
                                Description = job.Description,
                                Lcid = job.Language,
                                Url = job.RelativeUrl,
                                Template = job.BaseTemplate
                            }, job.InheritPermissions, job.InheritNavigation);
                        }
                        catch (Exception exception)
                        {
                            Log.LogError($"Error occured while creating subsite {job.RelativeUrl}. Inner exception: {exception.Message}");

                            if (parentWeb.WebExists(job.RelativeUrl))
                            {
                                parentWeb.DeleteWeb(job.RelativeUrl);
                            }

                            throw exception;
                        }

                        newWeb.Context.Load(newWeb);
                        newWeb.Context.ExecuteQueryRetry();
                    }

                    if (provisioningTemplate != null)
                    {
                        Log.LogInfo($"Applying provisioning template {provisioningTemplate.DisplayName}");
                        ApplyProvisioningTemplate(provisioningTemplate, newWeb);
                    }

                    Log.LogInfo($"Web {job.RelativeUrl} provisioned successfully.");
                }
            }
        }

        private ProvisioningTemplate GetProvisioningTemplate(string uri)
        {
            var templateRepository = new TemplateRepository();
            return templateRepository.GetTemplate(uri);
        }

        private void ApplyProvisioningTemplate(ProvisioningTemplate template, Web web)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("ApplyProvisioningTemplate"))
            {
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
