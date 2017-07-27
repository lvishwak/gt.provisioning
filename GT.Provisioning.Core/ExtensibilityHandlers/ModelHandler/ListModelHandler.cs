using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GT.Provisioning.Core.ExtensibilityHandlers.Definitions;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Diagnostics;
using GT.Provisioning.Core.ExtensibilityProviders.Definitions;
using OfficeDevPnP.Core.Utilities;

namespace GT.Provisioning.Core.ExtensibilityHandlers.ModelHandler
{
    internal class ListModelHandler : BaseModelHandler
    {
        public override void Deploy(Web web, BaseDefinition definition)
        {
            var webDefinition = definition as WebDefinition;
            if (webDefinition == null)
            {
                return;
            }

            using (PnPMonitoredScope scope = new PnPMonitoredScope("ListModelHandler.Deploy"))
            {
                var clientContext = web.Context;

                if (webDefinition.ListInstances != null && webDefinition.ListInstances.Any())
                {
                    foreach (var listInstance in webDefinition.ListInstances)
                    {
                        if (web.ListExists(listInstance.Title))
                        {
                            var list1 = web.GetListByTitle(listInstance.Title);
                            clientContext.ExecuteQuery();

                            list1.DeleteObject();
                            clientContext.ExecuteQueryRetry();
                        }

                        List list = web.CreateList(
                                (ListTemplateType)Enum.Parse(typeof(ListTemplateType), listInstance.TemplateId, ignoreCase: true),
                                listInstance.Title,
                                urlPath: listInstance.Url.StripInvalidUrlChars(),
                                enableVersioning: false);

                        clientContext.Load(list, l => l.RootFolder);
                        clientContext.ExecuteQueryRetry();

                        var rootFolder = list.RootFolder;
                        foreach (var folder in listInstance.Folders)
                        {
                            rootFolder.AddSubFolder(folder.Name);
                        }

                        clientContext.ExecuteQueryRetry();
                    }
                }
            }
        }
    }
}