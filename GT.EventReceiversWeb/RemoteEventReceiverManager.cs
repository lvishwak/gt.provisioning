using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;

namespace GT.EventReceiversWeb
{
    public class RemoteEventReceiverManager
    {
        private const string RECEIVER_NAME = "SyncContactEventReceiver";
        private const string SOURCE_LIST_TITLE = "Contacts";
        private const string DESTINATION_LIST_TITLE = "My Contacts";
        private const string TEAMSPACE_RELATIVE_URL = "teamspace";
        private const Int16 SEQUENCE_NUMBER = 1000;

        public void AssociateRemoteEventsToHostWeb(ClientContext clientContext)
        {
            bool rerExists = false;

            //Get the Title and EventReceivers lists
            clientContext.Load(clientContext.Web.Lists,
                lists => lists.Include(
                    list => list.Title,
                    list => list.EventReceivers).Where
                        (list => list.Title == SOURCE_LIST_TITLE));

            clientContext.ExecuteQuery();

            List contactList = clientContext.Web.Lists.FirstOrDefault();
            if (null == contactList)
            {
                //List does not exist
                return;
            }
            else
            {
                foreach (var rer in contactList.EventReceivers)
                {
                    if (rer.ReceiverName == RECEIVER_NAME)
                    {
                        rerExists = true;
                        Trace.WriteLine($"Found existing SyncContactEventReceiver receiver at {rer.ReceiverUrl}");
                    }
                }
            }

            if (!rerExists)
            {
#if DEBUG

                //Get WCF URL where this message was handled
                OperationContext op = OperationContext.Current;
                Message msg = op.RequestContext.RequestMessage;
#endif
                // 
                string remoteUrl 
                    = ($"https://{OperationContext.Current.Channel.LocalAddress.Uri.DnsSafeHost}/services/SyncContactEventReceiver.svc");

                EventReceiverDefinitionCreationInformation receiver =
                    new EventReceiverDefinitionCreationInformation()
                    {
                        EventType = EventReceiverType.ItemAdded,
                        ReceiverName = RECEIVER_NAME,
                        Synchronization = EventReceiverSynchronization.Synchronous,
                        ReceiverUrl = msg.Headers.To.ToString(),
                        SequenceNumber = SEQUENCE_NUMBER
                    };

                //Add the new event receiver to a list in the host web
                contactList.EventReceivers.Add(receiver);
                clientContext.ExecuteQuery();

                Trace.WriteLine($"Added ItemAdded receiver at {receiver.ReceiverUrl}");
            }
        }

        public void RemoveEventReceiversFromHostWeb(ClientContext clientContext)
        {
            List contactList = clientContext.Web.Lists.GetByTitle(SOURCE_LIST_TITLE);
            clientContext.Load(contactList, p => p.EventReceivers);
            clientContext.ExecuteQuery();

            var rer = contactList.EventReceivers.Where(
                e => e.ReceiverName == RECEIVER_NAME).FirstOrDefault();

            try
            {
                Trace.WriteLine($"Removing ItemAdded receiver at {rer.ReceiverUrl}");

                //This will fail when deploying via F5, but works when deployed to production
                rer.DeleteObject();
                clientContext.ExecuteQuery();
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
            }
        }

        public void ItemAddedToListEventHandler(ClientContext clientContext, Guid listId, int listItemId)
        {
            try
            {
                // get the source list
                List sourceList = clientContext.Web.Lists.GetById(listId);
                var fields = sourceList.Fields;
                sourceList.Context.Load(fields);
                sourceList.Context.ExecuteQuery();

                // get new added list item
                ListItem item = sourceList.GetItemById(listItemId);
                clientContext.Load(item);

                // get destination list
                clientContext.Load(clientContext.Web.Lists,
                lists => lists.Include(
                    list => list.Title).Where
                        (list => list.Title == DESTINATION_LIST_TITLE));
                clientContext.ExecuteQuery();

                List destinationList = clientContext.Web.Lists.FirstOrDefault();
                
                // copy list item to destination list
                if (destinationList != null)
                {
                    CopyItem(item, fields, destinationList);
                }

                // push changes
                clientContext.ExecuteQuery();
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
            }
        }

        private void CopyItem(ListItem sourceListItem, FieldCollection sourceFields, List destinationList)
        {
            var destinationListItem = destinationList.AddItem(new ListItemCreationInformation());

            foreach (Field field in sourceFields)
            {
                if (!field.ReadOnlyField
                    && !field.Hidden
                    && (field.InternalName != "Attachments")
                    && (field.InternalName != "ContentType"))
                {
                    destinationListItem[field.InternalName] = sourceListItem[field.InternalName];
                    destinationListItem.Update();
                }
            }
        }
    }
}