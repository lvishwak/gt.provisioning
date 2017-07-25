using GT.Provisioning.Core.Authentication;
using OfficeDevPnP.Core.Diagnostics;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System.IO;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;

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
                        
                        targetWeb.ApplyProvisioningTemplate(provisioningTemplate, applyingInfo);
                    }
                    catch (Exception exception)
                    {
                        Log.LogError(exception, $"Error occured while applying template to site {job.TargetSiteUrl}. Inner exception: {exception.Message}");
                    }
                }
            }
        }

        private ProvisioningTemplate GetProvisioningTemplateFromStream(Stream fileStream)
        {
            var schemaFormatter = XMLPnPSchemaFormatter.LatestFormatter;
            return schemaFormatter.ToProvisioningTemplate(fileStream);
        }
    }
}