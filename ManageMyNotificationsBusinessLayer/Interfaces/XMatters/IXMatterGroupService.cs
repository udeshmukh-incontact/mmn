using ManageMyNotificationsBusinessLayer.Data;

namespace ManageMyNotificationsBusinessLayer.Interfaces.XMatters
{
    public interface IXMatterGroupService
    {
        string GetGroupIdFromName(string groupName);

        GroupMembers GetGroupMembers(string groupId);

        Group CreateGroup(Group group);

        DeviceResponse AddDeviceToGroup(string groupId, string deviceId);

        GroupMember RemoveDeviceFromGroup(string groupId, string deviceId);

        ShiftResponse AddDeviceToShift(string groupId, string deviceId);

        GroupShifts GetShift(string groupId);

        GroupShift CreateShift(string groupId);
    }
}
