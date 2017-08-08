using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GT.Provisioning.Core.Jobs;
using GT.Provisioning.Core.Jobs.Handlers;
using OfficeDevPnP.Core.Diagnostics;

namespace GT.Provisioning.Core.Repositories
{
    public class ProvisioningRepository : IProvisioningRepository
    {
        public void Provision(ProvisioningJob provisioningJob)
        {
            var siteCollectionProvisioningJob = provisioningJob as SiteCollectionProvisioningJob;
            if (siteCollectionProvisioningJob != null)
            {
                ProvisionSiteCollection(siteCollectionProvisioningJob);
            }

            var subSiteProvisioningJob = provisioningJob as SubSiteProvisioningJob;
            if (subSiteProvisioningJob != null)
            {
                ProvisionSubSite(subSiteProvisioningJob);
            }
        }

        public void ApplyTemplate(ApplyTemplateProvisioningJob provisioningJob)
        {
            if (provisioningJob.JobId == Guid.Empty)
            {
                provisioningJob.JobId = Guid.NewGuid();
            }

            provisioningJob.CreationTimeStamp = DateTime.UtcNow;
            provisioningJob.JobType = provisioningJob.GetType().FullName;
            provisioningJob.Status = JobStatus.Pending;

            var applyTemplateJobHandler = new ApplyTemplateProvisioningJobHandler();
            applyTemplateJobHandler.RunJob(provisioningJob);
        }

        private void ProvisionSiteCollection(SiteCollectionProvisioningJob siteCollectionProvisioningJob)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("ProvisionSiteCollection"))
            {
                if (siteCollectionProvisioningJob.JobId == Guid.Empty)
                {
                    siteCollectionProvisioningJob.JobId = Guid.NewGuid();
                }

                siteCollectionProvisioningJob.StorageMaximumLevel = Constants.SiteProperties.StorageMaximumLevel;
                siteCollectionProvisioningJob.StorageWarningLevel = Constants.SiteProperties.StorageWarningLevel;
                siteCollectionProvisioningJob.CreationTimeStamp = DateTime.UtcNow;
                siteCollectionProvisioningJob.JobType = siteCollectionProvisioningJob.GetType().FullName;
                siteCollectionProvisioningJob.Status = JobStatus.Pending;

                var siteCollectionJobHandler = new SiteCollectionProvisioningJobHandler();
                siteCollectionJobHandler.RunJob(siteCollectionProvisioningJob);
            }
        }

        private void ProvisionSubSite(SubSiteProvisioningJob subSiteProvisioningJob)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("ProvisionSubSite"))
            {
                if (subSiteProvisioningJob.JobId == Guid.Empty)
                {
                    subSiteProvisioningJob.JobId = Guid.NewGuid();
                }

                subSiteProvisioningJob.CreationTimeStamp = DateTime.UtcNow;
                subSiteProvisioningJob.JobType = subSiteProvisioningJob.GetType().FullName;
                subSiteProvisioningJob.Status = JobStatus.Pending;

                var siteCollectionJobHandler = new SubSiteProvisioningJobHandler();
                siteCollectionJobHandler.RunJob(subSiteProvisioningJob);
            }
        }
    }
}