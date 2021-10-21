using InContact.Common.Utilities;
using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Proxy
{
    public interface ISalesforceNotificationServiceProxy
    {
        string GetXMPersonGuid(string adfsGuid);
        bool CreateXMPersonAndUpdateContacts(string contactId, string xmGuid);
        bool CreateXMPerson(string adfsGuid, string xmGuid);
        bool UpdateNotificationContacts(string adfsGuid, string xmGuid);
        bool UpdateContactWithXMPersonId(List<string> contactIds, string xmPersonId);
        bool DissociateNotificationContacts(string adfsGuid, string XmGuid);
        NotificationContact[] GetContactsByAdfsGuid(string adfsGuid);
        bool UpdateNotificationProfile(string adfsGuid, string xmGuid);

        bool UpdateSFContactsFirstnameLastName(string firstName, string lastName, string adfsGuid);
        bool AssociateProfiletoContact(string adfsGuid, string xmGuid);
    }
    public class SalesforceNotificationServiceProxy : BaseWrapper<SalesforceNotificationServiceClient, ISalesforceNotificationService>, ISalesforceNotificationServiceProxy
    {
        public string GetXMPersonGuid(string adfsGuid)
        {
            return Client.GetXMPersonGuid(adfsGuid);
        }
        public NotificationContact[] GetContactsByAdfsGuid(string adfsGuid)
        {
            return Client.GetContactsByAdfsGuid(adfsGuid);
        }
        public bool UpdateNotificationProfile(string adfsGuid, string xmGuid)
        {
            return Client.UpdateNotificationProfile(adfsGuid, xmGuid);
        }
        public bool UpdateSFContactsFirstnameLastName(string firstName, string lastName, string adfsGuid)
        {
            return Client.UpdateSFContactsFirstnameLastName(firstName, lastName, adfsGuid);
        }
        public bool CreateXMPerson(string adfsGuid, string xmGuid)
        {
            return Client.CreateXMPerson(adfsGuid, xmGuid);
        }
        public bool CreateXMPersonAndUpdateContacts(string contactId, string xmGuid)
        {
            return Client.CreateXMPersonAndUpdateContacts(contactId, xmGuid);
        }
        public bool UpdateNotificationContacts(string adfsGuid, string xmGuid)
        {
            return Client.UpdateNotificationContacts(adfsGuid, xmGuid);
        }
        public bool UpdateContactWithXMPersonId(List<string> contactIds, string xmPersonId)
        {
            return Client.UpdateContactWithXMPersonId(contactIds.ToArray(), xmPersonId);
        }
        public bool DissociateNotificationContacts(string adfsGuid, string XmGuid)
        {
            return Client.DissociateNotificationContacts(adfsGuid, XmGuid);
        }
        public bool AssociateProfiletoContact(string adfsGuid, string xmGuid)
        {
            return Client.AssociateProfiletoContact(adfsGuid, xmGuid);
        }
    }
}
