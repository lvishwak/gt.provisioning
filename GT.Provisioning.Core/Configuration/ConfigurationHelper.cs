using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.Configuration
{
    internal class ConfigurationHelper
    {
        public static IAppSettingConfiguration GetConfiguration
        {
            get { return new AppSettingConfiguration(); }
        }
    }
}
