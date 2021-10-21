using ManageMyNotificationsBusinessLayer.Data;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Interfaces
{
    public interface ICustomerNotificationsAPIService
    {
        SFContact GetContactByAdfsGuidOnACR(string adfsguid);
        bool UpdateACRContact(string contactId, SFContact contact);
        List<string> GetTopLevelAccount(string adfsguid);
    }
}
