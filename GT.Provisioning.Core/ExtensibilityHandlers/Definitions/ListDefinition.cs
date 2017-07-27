using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Core.ExtensibilityHandlers.Definitions
{
    public class ListDefinition : BaseDefinition
    {
        public String Title { get; set; }

        public String Url { get; set; }

        public String TemplateId { get; set; }

        public List<String> Properties { get; set; }

        public List<FolderDefinition> Folders { get; set; }
    }
}
