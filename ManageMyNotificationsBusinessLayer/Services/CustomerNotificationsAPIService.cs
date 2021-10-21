using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Interfaces;
using System.Collections.Generic;
using System.Net.Http;

namespace ManageMyNotificationsBusinessLayer.Services
{
    public class CustomerNotificationsAPIService : ICustomerNotificationsAPIService
    {
        private readonly ICustomerNotificationsAPIHelper _apiHelper;
        private List<Message> _auditMessages = new List<Message>();
        public CustomerNotificationsAPIService(ICustomerNotificationsAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        public SFContact GetContactByAdfsGuidOnACR(string adfsguid)
        {
            SFContact userNotifications = new SFContact();
            string url = $"v2/contact/GetContactByAdfsGuid/" + adfsguid;
            if (!string.IsNullOrWhiteSpace(adfsguid))
            {
                userNotifications = _apiHelper.CallApi<object, SFContact>(url, HttpMethod.Get, null)?.Result;
            }
            return userNotifications;
        }

        public bool UpdateACRContact(string contactId, SFContact contact)
        {
            string url = $"v2/contact/updateContact/" + contactId;
            var obj = new List<dynamic>();
            if (!string.IsNullOrWhiteSpace(contactId))
            {
                obj.Add(new { value = contact.FirstName, path = "FirstName" });
                obj.Add(new { value = contact.LastName, path = "LastName" });
                _apiHelper.CallApi<List<dynamic>, bool>(url, new HttpMethod("PATCH"), obj);
            }
            return false;
        }

        public List<string> GetTopLevelAccount(string adfsguid)
        {
            List<string> accountNos = new List<string>();
            string url = $"v2/account/GetTopLevelAccountByAdfsGuid?adfsGuid=" + adfsguid;
            if (!string.IsNullOrWhiteSpace(adfsguid))
            {
                accountNos = _apiHelper.CallApi<string, List<string>>(url, HttpMethod.Get, null)?.Result;
            }
            return accountNos;
        }
    }
}
