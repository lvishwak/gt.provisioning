using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Jobs
{
    public class SiteProvisioningJob : ProvisioningJob
    {
        /// <summary>
        /// Defines the Title of the Site Collection or Sub Site to provision
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Defines the Relative URL of the Site Collection or Sub Site to provision
        /// </summary>
        public string RelativeUrl { get; set; }

        /// <summary>
        /// Defines the Description for the Site Collection or Sub site to provision
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Defines the PnP Provisioning Template to use for the Site Collection or Sub site to provision
        /// </summary>
        public string PnPTemplate { get; set; }

        /// <summary>
        /// Defines the base template for the Site Collection or Sub site to provision.
        /// </summary>
        public string BaseTemplate { get; set; }

        /// <summary>
        /// Defines the Language for the Site Collection or Sub site to provision
        /// </summary>
        public uint Language { get; set; }

        /// <summary>
        /// Defines the TimeZone for the Site Collection or Sub site to provision
        /// </summary>
        public Int32 TimeZone { get; set; }

        /// <summary>
        /// Defines the Parameters keys and values for the Site Collection or Sub site to provision
        /// </summary>
        public Dictionary<String, String> TemplateParameters { get; set; }

        /// <summary>
        /// Defines the owners for site
        /// </summary>
        public List<string> Owners { get; set; }        
    }
}
