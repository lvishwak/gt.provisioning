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

        private void ProvisionSiteCollection(ProvisioningJob provisioningJob)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("ProvisionSiteCollection"))
            {
                var siteCollectionProvisioningJob = provisioningJob as SiteCollectionProvisioningJob;
                if (siteCollectionProvisioningJob == null)
                {
                    // throw exception
                }

                ProvisioningJobHandler siteCollectionJobHandler
                    = new SiteCollectionProvisioningJobHandler();

                if (siteCollectionProvisioningJob.JobId == Guid.Empty)
                {
                    siteCollectionProvisioningJob.JobId = Guid.NewGuid();
                }

                siteCollectionProvisioningJob.StorageWarningLevel = 70;
                siteCollectionProvisioningJob.StorageMaximumLevel = 100;
                siteCollectionProvisioningJob.CreationTimeStamp = DateTime.UtcNow;
                siteCollectionProvisioningJob.JobType
                    = siteCollectionProvisioningJob.GetType().FullName;
                siteCollectionProvisioningJob.Status = JobStatus.Pending;

                siteCollectionJobHandler.RunJob(provisioningJob);
            }
        }

        private void ProvisionSubSite(ProvisioningJob subSiteProvisioningJob)
        {
            using (PnPMonitoredScope Log = new PnPMonitoredScope("ProvisionSubSite"))
            {
                ProvisioningJobHandler siteCollectionJobHandler
                    = new SubSiteProvisioningJobHandler();

                if (subSiteProvisioningJob.JobId == Guid.Empty)
                {
                    subSiteProvisioningJob.JobId = Guid.NewGuid();
                }

                subSiteProvisioningJob.CreationTimeStamp = DateTime.UtcNow;
                subSiteProvisioningJob.JobType
                    = subSiteProvisioningJob.GetType().FullName;
                subSiteProvisioningJob.Status = JobStatus.Pending;

                siteCollectionJobHandler.RunJob(subSiteProvisioningJob);
            }
        }
    }
}