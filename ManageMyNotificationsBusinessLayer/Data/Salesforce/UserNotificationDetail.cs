using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Data.Salesforce
{
    public class UserNotificationDetail
    {
        public string XMGuid { get; set; }
        public string LanguagePreference { get; set; }
        public List<NotificationContact> NotificationContacts { get; set; }
        public Dictionary<string, XMUserDetails> Events_Notifications { get; set; }
        public Dictionary<string, XMUserDetails> Maintainance_Notifications { get; set; }
    }
}
