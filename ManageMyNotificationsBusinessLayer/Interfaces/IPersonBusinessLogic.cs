using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Data.Salesforce;
using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Interfaces
{
    public interface IPersonBusinessLogic
    {
        UserNotificationDetail GetSFNotificationProfile(string adfsGuid);
        //string GetXMGuid(string adfsGuid);
        //bool ProfileEnableStatus(string adfsGuid);
        //bool IsDeviceAvailable(string adfsGuid);
        //bool SyncSFXMattersProfile(string adfsGuid, string xmGuid, GetPersonResponse person);
        bool SyncSFXMattersProfile(List<NotificationContact> sfContacts, GetPersonResponse person);
        //GetPersonResponse GetSfContact(string adfsGuid);
        GetPersonResponse GetSfContact(List<NotificationContact> sfContacts);
        Person CreateXmattersPerson(string adfsGuid, Person person);
        void EnableXMattersProfile(string xmGuid);
        void UpdateNotificationProfile(string adfsGuid, string xmGuid);

        bool UpdateSFContactsFirstnameLastName(string firstName,string lastName, string adfsGuid);

        bool CreateSalesforceprofile(string adfsGuid, string XmGuid);
        //bool AssociateProfiletoContact(string AdfsGuid, string XmGuid);
        //bool UpdateSFContacts(string AdfsGuid, string XmGuid);
        //List<NotificationGroup> GetPersonNotificationGroups(GetPersonResponse person, PersonDevices devices, bool returnAll = false, bool isNewUser = false, string adfsGuid = "");
        List<NotificationGroup> GetPersonNotificationGroups(GetPersonResponse person, PersonDevices devices, GroupMembers groupMembers,
            Dictionary<string, XMUserDetails> events_Notifications,
            Dictionary<string, XMUserDetails> maintainance_Notifications,
            List<NotificationContact> sfContacts);
        DeviceResponse CreateDevice(DeviceRequest deviceToAdd);
        DeviceResponse RemoveDevice(DeviceRequest device);
        bool DisableXMattersProfile(string adfsGuid, string xmGuid);
        //string GetXMattersSFProfileGuid(string adfsGuid);
        //bool DissociateSFContacts(string adfsGuid, string XmGuid);
        //bool IsXmatterPersonActive(string xmGuid);
        PersonDevices GetPersonDevices(string xmGuid);
        GetPersonResponse GetPerson(string xmGuid);
        GroupMembers GetGroups(string xmGuid, bool returnAll = false);
        void SavePersonNotificationGroups(List<NotificationGroup> oldGroups, List<NotificationGroup> newGroups);
        void AddDeviceToShift(string groupId, string deviceId);
        bool UpdatePersonTimeZone(Person person);
        event OnPersonDataSaveHandler OnPersonDataSaved;
        event OnDeviceSaveHandler OnDeviceSaved;
        event OnGroupsSaveHandler OnNotificationGroupSaved;
        GetPersonResponse GetPersonByTargetName(string targetName);
        List<string> GetTopLevelAccount(string adfsguid);
    }
}
