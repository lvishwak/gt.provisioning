using GT.Provisioning.Core.ExtensibilityHandlers.Definitions;
using Microsoft.SharePoint.Client;

namespace GT.Provisioning.Core.ExtensibilityHandlers.ModelHandler
{
    internal abstract class BaseModelHandler
    {
        public abstract void Deploy(Web web, BaseDefinition definition);
    }
}