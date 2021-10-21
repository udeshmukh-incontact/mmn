using ManageMyNotificationsBusinessLayer.Data;

namespace ManageMyNotificationsBusinessLayer.Interfaces.XMatters
{
    public interface IXMatterPersonService
    {
        GetPersonResponse GetPerson(string xMatterId);
        PersonDevices GetPersonDevices(string xMatterId);
        GroupMembers GetPersonGroups(string xMatterId, bool returnAll = false);
        Person CreatePerson(Person person);
        Person UpdatePerson(Person person);
        DeviceResponse CreateDevice(DeviceRequest device);
        Person AddPersonToGroup(string groupId, string personId);
        DeviceResponse RemoveDevice(string deviceId);
        GetPersonResponse GetPersonByTargetName(string targetName);
    }
}
