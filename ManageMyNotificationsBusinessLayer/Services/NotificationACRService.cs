using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ManageMyNotificationsBusinessLayer.Services
{
    public class NotificationACRService : INotificationACRService
    {
        private readonly ICustomerNotificationsAPIHelper _apiHelper;
        private List<Message> _auditMessages = new List<Message>();
        public NotificationACRService(ICustomerNotificationsAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        public SFContact GetConactByAdfsGuidOnACR(string adfsguid)
        {
            SFContact userNotifications = new SFContact();
            string url = $"v2/contact/GetContactByAdfsGuid/" + adfsguid;
            if (!string.IsNullOrWhiteSpace(adfsguid))
            {
                userNotifications = _apiHelper.CallApi<object, SFContact>(url, HttpMethod.Get, null);
            }
            return userNotifications;

        }

        public bool UpdateACRContact(string contactId,SFContact contact)
        {
            //SFContact userNotifications = new SFContact();
            string url = $"v2/contact/updateContact/"+ contactId;
            var obj = new List<dynamic>();
            if (!string.IsNullOrWhiteSpace(contactId))
            {
                obj.Add(new { value = contact.FirstName, path = "FirstName" });
                obj.Add(new { value = contact.LastName, path = "LastName" });
                _apiHelper.CallApi<List<dynamic>, bool>(url, new HttpMethod("PATCH"), obj);
            }
            return false;

        }
    }
}
