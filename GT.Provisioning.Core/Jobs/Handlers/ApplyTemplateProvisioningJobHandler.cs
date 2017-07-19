using GT.Provisioning.Core.Authentication;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

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
        }

        private void ApplyTemplate(ApplyTemplateProvisioningJob job)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("ApplyTemplate"))
            {
                using (var appOnlyClientContext = AppOnlyContextProvider.GetAppOnlyClientContext(job.TargetSiteUrl))
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

                        targetWeb.ApplyProvisioningTemplate(job.PnPTemplate, applyingInfo);
                    }
                    catch (Exception exception)
                    {
                        Log.LogError(exception, $"Error occured while applying template to site {job.TargetSiteUrl}");
                    }
                }
            }
        }
    }
}