using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Interfaces.XMatters;
using System.Configuration;
using System.Net.Http;

namespace ManageMyNotificationsBusinessLayer.Services.XMatters
{
    public class XMatterGroupService : IXMatterGroupService
    {
        private readonly IXMattersServiceHelper _xMattersHelper;

        public XMatterGroupService(IXMattersServiceHelper xMattersHelper)
        {
            _xMattersHelper = xMattersHelper;
        }

        public string GetGroupIdFromName(string groupName)
        {
            var url = $"groups?search={groupName}&fields=name";

            // Get all groups with Name LIKE queried group name
            var groups = _xMattersHelper.CallXMatters<object, Groups>(url, HttpMethod.Get);

            // Get the group with exact matching Name
            var group = groups?.Result.GroupMember.Find(i => i.TargetName == groupName);
            if (group != null)
            {
                return group.Id;
            }

            return "";
        }

        public GroupMembers GetGroupMembers(string groupId)
        {
            return _xMattersHelper.CallXMatters<object, GroupMembers>($"groups/{groupId}/members", HttpMethod.Get)?.Result;
        }

        public Group CreateGroup(Group group)
        {
            return _xMattersHelper.CallXMatters<Group, Group>("groups", HttpMethod.Post, group)?.Result;
        }
        
        public DeviceResponse AddDeviceToGroup(string groupId, string deviceId)
        {
            string url = $"groups/{groupId}/members";

            var body = new { id = deviceId, recipientType = "DEVICE" };

            return _xMattersHelper.CallXMatters<object, DeviceResponse>(url, HttpMethod.Post, body)?.Result;
        }

        public ShiftResponse AddDeviceToShift(string groupId, string deviceId)
        {
            string shiftName = ConfigurationManager.AppSettings["24x7Shift"];
            string url = $"groups/{groupId}/shifts/{shiftName}/members";

            var body = new { recipient = new { id = deviceId, recipientType = "DEVICE" } };

           return _xMattersHelper.CallXMatters<object, ShiftResponse>(url, HttpMethod.Post, body)?.Result;
           
        }
        public GroupShifts GetShift(string groupId)
        {
            return _xMattersHelper.CallXMatters<object, GroupShifts>($"groups/{groupId}/shifts/", HttpMethod.Get)?.Result;
        }
        public GroupShift CreateShift(string groupId)
        {
            string shiftName = ConfigurationManager.AppSettings["24x7Shift"];
            string url = $"groups/{groupId}/shifts";

            var body = new { name = shiftName };

            return _xMattersHelper.CallXMatters<object, GroupShift>(url, HttpMethod.Post, body)?.Result;
           
        }

        public GroupMember RemoveDeviceFromGroup(string groupId, string deviceId)
        {
            string url = $"groups/{groupId}/members/{deviceId}";

            return _xMattersHelper.CallXMatters<object, GroupMember>(url, HttpMethod.Delete)?.Result;
        }
    }
}
