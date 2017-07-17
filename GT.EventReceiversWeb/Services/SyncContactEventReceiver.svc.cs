using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

namespace GT.EventReceiversWeb.Services
{
    public class SyncContactEventReceiver : IRemoteEventService
    {
        /// <summary>
        /// Handles events that occur before an action occurs, such as when a user adds or deletes a list item.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        /// <returns>Holds information returned from the remote event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();

            switch (properties.EventType)
            {
                case SPRemoteEventType.AppInstalled:
                    HandleAppInstalled(properties);
                    break;
                case SPRemoteEventType.AppUninstalling:
                    HandleAppUninstalling(properties);
                    break;
                case SPRemoteEventType.ItemAdded:
                    HandleItemAdded(properties);
                    break;
                case SPRemoteEventType.ItemUpdated:
                    HandleItemUpdate(properties);
                    break;
            }

            return result;
        }        

        /// <summary>
        /// Handles events that occur after an action occurs, such as after a user adds an item to a list or deletes an item from a list.
        /// </summary>
        /// <param name="properties">Holds information about the remote event.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            switch (properties.EventType)
            {
                case SPRemoteEventType.ItemAdded:
                    HandleItemAdded(properties);
                    break;
            }
        }

        /// <summary>
        /// Handles when an app is installed. it attaches a remote event receiver to the list.  
        /// </summary>
        /// <param name="properties"></param>
        private void HandleAppInstalled(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext =
                TokenHelper.CreateAppEventClientContext(properties, false))
            {
                if (clientContext != null)
                {
                    new RemoteEventReceiverManager().AssociateRemoteEventsToHostWeb(clientContext);
                }
            }
        }

        /// <summary>
        /// Removes the remote event receiver from the list
        /// </summary>
        /// <param name="properties"></param>
        private void HandleAppUninstalling(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext =
                TokenHelper.CreateAppEventClientContext(properties, false))
            {
                if (clientContext != null)
                {
                    new RemoteEventReceiverManager().RemoveEventReceiversFromHostWeb(clientContext);
                }
            }
        }

        // <summary>
        /// Handles the ItemAdded event 
        /// </summary>
        /// <param name="properties"></param>
        private void HandleItemAdded(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext =
                TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (clientContext != null)
                {
                    new RemoteEventReceiverManager().ItemAddedToListEventHandler(clientContext, properties.ItemEventProperties.ListId, properties.ItemEventProperties.ListItemId);
                }
            }
        }

        // <summary>
        /// Handles the ItemUpdated event
        /// </summary>
        /// <param name="properties"></param>
        private void HandleItemUpdate(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext =
                TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (clientContext != null)
                {
                    new RemoteEventReceiverManager().ItemAddedToListEventHandler(clientContext, properties.ItemEventProperties.ListId, properties.ItemEventProperties.ListItemId);
                }
            }
        }
    }
}