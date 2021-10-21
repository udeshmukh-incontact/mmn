using System;
using System.Collections.Generic;
using System.Net.Http;
using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Interfaces.XMatters;

namespace ManageMyNotificationsBusinessLayer.Services.XMatters
{
    public class XMatterPersonService : IXMatterPersonService
    {
        private readonly IXMattersServiceHelper _xMattersHelper;
        public XMatterPersonService(IXMattersServiceHelper xMattersHelper)
        {
            _xMattersHelper = xMattersHelper;
        }

        public GetPersonResponse GetPerson(string xMatterId)
        {
            GetPersonResponse person = null;

            if (Guid.TryParse(xMatterId, out var guidId))
            {
                var url = $"people/{xMatterId}?embed=roles";
                person = _xMattersHelper.CallXMatters<object, GetPersonResponse>(url, HttpMethod.Get)?.Result;
            }
            return person;
        }

        public PersonDevices GetPersonDevices(string xMatterId)
        {
            PersonDevices personDevices = null;

            if (Guid.TryParse(xMatterId, out var guidId))
            {
                var url = $"people/{xMatterId}/devices?embed=timeframes";
                personDevices = _xMattersHelper.CallXMatters<object, PersonDevices>(url, HttpMethod.Get)?.Result;
            }
            return personDevices;
        }

        public GroupMembers GetPersonGroups(string xMatterId, bool returnAll = false)
        {
            GroupMembers personGroupsTotal = new GroupMembers();
            personGroupsTotal.GroupMember = new List<GroupMember>();
            personGroupsTotal.Total = 0;
            GroupMembers personGroups = null;

            if (Guid.TryParse(xMatterId, out var guidId))
            {
                var url = $"people/{xMatterId}/group-memberships";
                do
                {
                    personGroups = _xMattersHelper.CallXMatters<object, GroupMembers>(url, HttpMethod.Get)?.Result;
                    if (personGroups == null)
                        return personGroupsTotal;
                    personGroupsTotal.GroupMember.AddRange(personGroups.GroupMember);
                    personGroupsTotal.Total += personGroups.Total;
                    personGroupsTotal.Count += personGroups.Count;
                    url = (personGroups.links != null && personGroups.links.Next !=null) ? personGroups.links.Next.Replace("/api/xm/1/", "") : "";
                } while (returnAll && url != "");
            }
            return personGroupsTotal;
        }

        public Person CreatePerson(Person person)
        {
            const string url = "people";
            Person personCreated = null;
            if (person != null)
            {
                personCreated = _xMattersHelper.CallXMatters<Person, Person>(url, HttpMethod.Post, person)?.Result;
            }
            return personCreated;
        }

        public Person UpdatePerson(Person person)
        {
            if (person != null)
            {
                const string url = "people";
                person = _xMattersHelper.CallXMatters<Person, Person>(url, HttpMethod.Post, person)?.Result;
            }
            return person;
        }

        public DeviceResponse CreateDevice(DeviceRequest device)
        {
            const string url = "devices";
            DeviceResponse deviceCreated = null;

            if (device != null)
            {
                deviceCreated = _xMattersHelper.CallXMatters<DeviceRequest, DeviceResponse>(url, HttpMethod.Post, device)?.Result;
            }
            return deviceCreated;
        }

        public Person AddPersonToGroup(string groupId, string personId)
        {
            var url = string.Format("groups/{0}/members", groupId);  // The Url specifies the group to update
            Person person = null;
            if (!string.IsNullOrWhiteSpace(personId))
            {

                var postObject = new { id = personId, recipientType = "PERSON" };  // post object determines.
                person = _xMattersHelper.CallXMatters<Object, Person>(url, HttpMethod.Post, postObject)?.Result;

            }
            return person;

        }

        public DeviceResponse RemoveDevice(string deviceId)
        {

           var url= string.Format("devices/{0}", deviceId);
            DeviceResponse deviceRemoved = null;

            if (deviceId != null)
            {
                deviceRemoved = _xMattersHelper.CallXMatters<DeviceRequest, DeviceResponse>(url, HttpMethod.Delete)?.Result;
            }
            return deviceRemoved;
        }

        public GetPersonResponse GetPersonByTargetName(string targetName)
        {
            GetPersonResponse person = null;
            var url = $"people/{targetName}?embed=roles";
            person = _xMattersHelper.CallXMatters<object, GetPersonResponse>(url, HttpMethod.Get)?.Result;
            return person;
        }

    }
}
