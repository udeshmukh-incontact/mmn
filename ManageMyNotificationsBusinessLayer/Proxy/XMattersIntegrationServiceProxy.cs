using InContact.Common.Utilities;
using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Proxy
{
    public interface IXMattersIntegrationServiceProxy
    {
        Dictionary<string, XMUserDetails> GetXMProfileDetails(string contactId, string eventType);
    }
    
    public class XMattersIntegrationServiceProxy : BaseWrapper<XMattersIntegrationServiceClient, IXMattersIntegrationService>, IXMattersIntegrationServiceProxy
    {
        public Dictionary<string, XMUserDetails> GetXMProfileDetails(string contactId, string eventType)
        {
            return Client.GetXMProfileDetails(contactId, eventType);
        }
    }
}
