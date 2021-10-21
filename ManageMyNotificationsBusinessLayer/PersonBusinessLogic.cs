using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Data.Salesforce;
using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using ManageMyNotificationsBusinessLayer.Interfaces;
using ManageMyNotificationsBusinessLayer.Interfaces.XMatters;
using ManageMyNotificationsBusinessLayer.Proxy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ManageMyNotificationsBusinessLayer
{
    public delegate void OnPersonDataSaveHandler(string type, Person person);
    public delegate void OnDeviceSaveHandler(string type, DeviceRequest device);
    public delegate void OnGroupsSaveHandler(string type, NotificationGroupSave notificationGroup);
    public class PersonBusinessLogic : IPersonBusinessLogic
    {
        private readonly IXMatterPersonService _xmatterPersonService;
        private readonly IXMatterGroupService _xmatterGroupService;
        private readonly ISalesforceNotificationServiceProxy _sfNotificationService;
        private readonly IXMattersIntegrationServiceProxy _xMattersIntegrationService;
        private readonly ICustomerNotificationsAPIService _notificationACRService;
        static object lockShiftCreation = new object();
        private bool? _isiAIMVer2Enabled = null;
        public event OnPersonDataSaveHandler OnPersonDataSaved;
        public event OnDeviceSaveHandler OnDeviceSaved;
        public event OnGroupsSaveHandler OnNotificationGroupSaved;

        public PersonBusinessLogic(IXMatterPersonService personService, ISalesforceNotificationServiceProxy sfNotificationService, IXMattersIntegrationServiceProxy xMattersIntegrationService, IXMatterGroupService groupService, ICustomerNotificationsAPIService notificationACRService = null)
        {
            _xmatterPersonService = personService;
            _sfNotificationService = sfNotificationService;
            _xMattersIntegrationService = xMattersIntegrationService;
            _xmatterGroupService = groupService;
            _notificationACRService = notificationACRService;
        }
        public UserNotificationDetail GetSFNotificationProfile(string adfsGuid)
        {
            UserNotificationDetail detail = new UserNotificationDetail();
            try
            {
                if (IsiAIMVer2Enabled)
                {
                    detail.NotificationContacts = new List<NotificationContact>();
                    var contact = _notificationACRService.GetContactByAdfsGuidOnACR(adfsGuid);
                    detail.NotificationContacts.Add(contact.ToViewModel());
                    detail.LanguagePreference = contact.LanguagePreference;
                }
                else
                {
                    detail.NotificationContacts = _sfNotificationService.GetContactsByAdfsGuid(adfsGuid).Where(c => !c.Deactivated).ToList();
                }
                if (detail.NotificationContacts.Count > 0)
                {
                    NotificationContact primaryContact = detail.NotificationContacts.First();
                    detail.XMGuid = primaryContact.XMPersonGuid;
                }

                detail.Events_Notifications = new Dictionary<string, XMUserDetails>();
                detail.Maintainance_Notifications = new Dictionary<string, XMUserDetails>();
                var contactWithXMGuid = detail.NotificationContacts.Where(x => !string.IsNullOrWhiteSpace(x.XMPersonGuid)).FirstOrDefault()?.Id;
                if (!string.IsNullOrWhiteSpace(contactWithXMGuid))
                {
                    detail.Events_Notifications = _xMattersIntegrationService.GetXMProfileDetails(contactWithXMGuid, "eventType");
                    detail.Maintainance_Notifications = _xMattersIntegrationService.GetXMProfileDetails(contactWithXMGuid, "MaintType");
                }
                var contactWithoutXMGuid = detail.NotificationContacts.Where(x => string.IsNullOrWhiteSpace(x.XMPersonGuid)).Select(c => c.Id);
                if (contactWithoutXMGuid.Count() > 0)
                {
                    foreach (var contactId in contactWithoutXMGuid)
                    {
                        Helper.MergeRangeToAccount(detail.Events_Notifications, _xMattersIntegrationService.GetXMProfileDetails(contactId, "eventType"));
                        Helper.MergeRangeToAccount(detail.Maintainance_Notifications, _xMattersIntegrationService.GetXMProfileDetails(contactId, "MaintType"));
                    }
                }
            }
            catch (Exception ex) when (ex.InnerException is EndpointNotFoundException)
            {
                throw new ServiceException(Constants.ErrorMessage.SALESFORCE_SERVICE_DOWN, ex);
            }
            catch (Exception ex)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("adfsGuid", adfsGuid);
                string errorMessage = ErrorMessageFormatter.FormatElmahErrorMessage(ex.Message, parameters);
                throw new Exception(errorMessage, ex);
            }
            return detail;
        }

        public bool SyncSFXMattersProfile(List<NotificationContact> sfContacts, GetPersonResponse person)
        {
            bool isEnabled = false;

            if (person != null)
            {
                List<string> contactIds = sfContacts.Where(c => string.IsNullOrWhiteSpace(c.XMPersonId)).Select(c => c.Id).ToList();
                if (contactIds.Count > 0)
                {
                    string xmPersonId = sfContacts.Where(c => !string.IsNullOrWhiteSpace(c.XMPersonId)).Select(c => c.XMPersonId).First();
                    UpdateSFContacts(contactIds, xmPersonId);
                    isEnabled = true;
                }
            }
            return isEnabled;
        }

        public GetPersonResponse GetSfContact(List<NotificationContact> sfContacts)
        {
            if (sfContacts != null)
            {
                GetPersonResponse personToCreate = new GetPersonResponse();
                var sfContact = sfContacts.Count > 0 ? sfContacts.First() : null;
                if (sfContact == null)
                {
                    return null;
                }
                personToCreate.TargetName = sfContact.Id;
                personToCreate.FirstName = sfContact.FirstName;
                personToCreate.LastName = sfContact.LastName;

                return personToCreate;
            }
            else
            {
                return null;
            }
        }

        public Person CreateXmattersPerson(string adfsGuid, Person person)
        {
            string xmGuid = null;
            if (person != null)
            {
                person = _xmatterPersonService.CreatePerson(person);
                OnPersonDataSaved?.Invoke(Constants.AuditType.EDIT_XMATTERS_USERDETAILS, person);
                xmGuid = person.Id.ToString();
                CreateSalesforceprofile(person.TargetName, xmGuid);
            }
            return person;
        }

        public GetPersonResponse GetPersonByTargetName(string targetName)
        {
            GetPersonResponse person = null;
            if (!string.IsNullOrEmpty(targetName))
            {
                person = _xmatterPersonService.GetPersonByTargetName(targetName);
            }
            return person;
        }

        public void EnableXMattersProfile(string xmGuid)
        {
            Person personToUpdate = new Person
            {
                Id = new Guid(xmGuid),
                Status = "ACTIVE"
            };
            Person updatedPerson = _xmatterPersonService.UpdatePerson(personToUpdate);
            OnPersonDataSaved?.Invoke(Constants.AuditType.ENABLED_NOTIFICATION_PROFILE, updatedPerson);
        }

        public void UpdateNotificationProfile(string adfsGuid, string xmGuid)
        {
            try
            {
                _sfNotificationService.UpdateNotificationProfile(adfsGuid, xmGuid);
            }
            catch (Exception ex) when (ex.InnerException is EndpointNotFoundException)
            {
                throw new ServiceException(Constants.ErrorMessage.SALESFORCE_SERVICE_DOWN, ex);
            }
            catch (Exception ex)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("adfsGuid", adfsGuid);
                parameters.Add("XmGuid", xmGuid);
                string errorMessage = ErrorMessageFormatter.FormatElmahErrorMessage(ex.Message, parameters);
                throw new Exception(errorMessage, ex);
            }
        }

        public bool CreateSalesforceprofile(string adfsGuid, string XmGuid)
        {
            bool updatevalue = false;
            try
            {
                updatevalue = _sfNotificationService.CreateXMPersonAndUpdateContacts(adfsGuid, XmGuid);
            }
            catch (Exception ex) when (ex.InnerException is EndpointNotFoundException)
            {
                throw new ServiceException(Constants.ErrorMessage.SALESFORCE_SERVICE_DOWN, ex);
            }
            catch (Exception ex)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("contactId", adfsGuid);
                parameters.Add("XmGuid", XmGuid);
                string errorMessage = ErrorMessageFormatter.FormatElmahErrorMessage(ex.Message, parameters);
                throw new Exception(errorMessage, ex);
            }
            return updatevalue;
        }

        private bool UpdateSFContacts(List<string> contactIds, string xmPersonId)
        {
            bool UpdateValue = false;
            try
            {
                UpdateValue = _sfNotificationService.UpdateContactWithXMPersonId(contactIds, xmPersonId);
            }
            catch (Exception ex) when (ex.InnerException is EndpointNotFoundException)
            {
                throw new ServiceException(Constants.ErrorMessage.SALESFORCE_SERVICE_DOWN, ex);
            }
            catch (Exception ex)
            {
            }
            return UpdateValue;
        }
        public bool UpdateSFContactsFirstnameLastName(string firstName, string lastName, string adfsGuid)
        {
            bool UpdateValue = false;
            try
            {
                if (IsiAIMVer2Enabled)
                {
                    var acrContact = _notificationACRService.GetContactByAdfsGuidOnACR(adfsGuid).ToViewModel();
                    SFContact contact = new SFContact() { FirstName = firstName, LastName = lastName };
                    UpdateValue = _notificationACRService.UpdateACRContact(acrContact.Id, contact);
                }
                else
                {
                    UpdateValue = _sfNotificationService.UpdateSFContactsFirstnameLastName(firstName, lastName, adfsGuid);
                }
            }
            catch (Exception ex) when (ex.InnerException is EndpointNotFoundException)
            {
                throw new ServiceException(Constants.ErrorMessage.SALESFORCE_SERVICE_DOWN, ex);
            }
            catch (Exception ex)
            {
            }
            return UpdateValue;
        }
        public List<NotificationGroup> GetPersonNotificationGroups(GetPersonResponse person, PersonDevices devices, GroupMembers groupMembers,
            Dictionary<string, XMUserDetails> events_Notifications,
            Dictionary<string, XMUserDetails> maintainance_Notifications,
            List<NotificationContact> sfContacts)
        {
            var notificationGroups = new List<NotificationGroup>();
            try
            {
                var deviceNotifications = GetDevicesForNotificationGrop(devices);

                NotificationGroup notificationGroup;

                foreach (var item in events_Notifications.Values)
                {
                    var index = 0;
                    foreach (var account in item.accounts)
                    {
                        notificationGroup = new NotificationGroup()
                        {
                            ParentAccountIDs = GetParentBUIDs(account.Id, sfContacts.ToArray()),
                            AccountBU = $"{account.Name} {account.CadebillAccountNo__c}",
                            ProductCluster = item.ClusterPrefix.ToUpper().Equals("NONE") ? item.Product : $"{item.Product} / {item.ClusterPrefix}",
                            PartnerPrefix = item.PartnerPrefix,
                            EventsGroupName = item.XmattersGroupName
                        };

                        XMUserDetails xmUserDetails = maintainance_Notifications.Values
                                                    .Where(m => m.Product == item.Product &&
                                                                m.ClusterPrefix == item.ClusterPrefix &&
                                                                m.accounts.Count() == item.accounts.Count())
                                                    .FirstOrDefault(m => m.accounts[index].CadebillAccountNo__c == item.accounts[index].CadebillAccountNo__c);
                        if (xmUserDetails != null)
                            notificationGroup.MaintainanceGroupName = xmUserDetails.XmattersGroupName;
                        // Clone List of Devices with subscribed notification group
                        notificationGroup.DeviceNotifications = deviceNotifications.ConvertAll(i => new DeviceNotifications()
                        {
                            DeviceId = i.DeviceId,
                            DeviceType = i.DeviceType,
                            IsEventsSubscribed = GetNotificationGroupStatus(groupMembers, i.DeviceId, notificationGroup.EventsGroupName),
                            IsMaintainanceSubscribed = GetNotificationGroupStatus(groupMembers, i.DeviceId, notificationGroup.MaintainanceGroupName)
                        });
                        notificationGroups.Add(notificationGroup);
                        index++;
                    }
                }

                return notificationGroups;
            }
            catch (Exception ex)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("person", "{Id: " + person.Id + ", FirstName: " + person.FirstName + ", LastName: " + person.LastName + "}");
                string errorMessage = ErrorMessageFormatter.FormatElmahErrorMessage(ex.Message, parameters);
                throw new Exception(errorMessage, ex);
            }
        }
        //*Code Clean up Part*//
        private List<string> GetParentBUIDs(string id, NotificationContact[] accounts)
        {
            if (!string.IsNullOrEmpty(id))
            {
                foreach (var account in accounts)
                {
                    if (id.ToLower().Equals(account.AccountId.ToLower()))
                    {
                        if (account.AccountRootParentIDs != null)
                            return new List<string>(account.AccountRootParentIDs);
                    }
                }
            }
            return new List<string>();
        }

        /// <summary>
        /// add device to xMatters when deviceId is NULL
        /// Updated Device to xMatters when deviceId is provided.
        /// </summary>
        /// <param name="createdPerson"></param>
        /// <param name="deviceToAdd"></param>
        /// <returns></returns>
        public DeviceResponse CreateDevice(DeviceRequest deviceToAdd)
        {
            DeviceResponse deviceResponse = _xmatterPersonService.CreateDevice(deviceToAdd);
            if (string.IsNullOrEmpty(deviceToAdd.Id))
                OnDeviceSaved?.Invoke(Constants.AuditType.ADDED_NOTIFICATION_DEVICE, deviceToAdd);
            else
                OnDeviceSaved?.Invoke(Constants.AuditType.UPDATED_NOTIFICATION_DEVICE, deviceToAdd);
            return deviceResponse;
        }

        public DeviceResponse RemoveDevice(DeviceRequest device)
        {
            DeviceResponse deviceResponse = _xmatterPersonService.RemoveDevice(device.Id);
            OnDeviceSaved?.Invoke(Constants.AuditType.REMOVED_NOTIFICATION_DEVICE, device);
            return deviceResponse;

        }

        public bool DisableXMattersProfile(string adfsGuid, string xmGuid)
        {
            bool IsDisabled = false;
            Person updatedPerson = null;
            List<string> ObjRoles = new List<string>();
            ObjRoles.Add("No Access User");
            ObjRoles.Add("Standard User");
            if (!string.IsNullOrEmpty(xmGuid))
            {
                updatedPerson = new Person
                {
                    Id = new Guid(xmGuid),
                    Status = "INACTIVE",
                    roles = ObjRoles
                };
                var result = _xmatterPersonService.UpdatePerson(updatedPerson);

                //if (result != null)
                //{
                //    DissociateSFContacts(adfsGuid, xmGuid);
                //}
                IsDisabled = true;
            }
            OnPersonDataSaved?.Invoke(Constants.AuditType.DISABLED_NOTIFICATION_PROFILE, updatedPerson);
            return IsDisabled;
        }

        public PersonDevices GetPersonDevices(string xmGuid)
        {
            PersonDevices personDevices = null;
            personDevices = _xmatterPersonService.GetPersonDevices(xmGuid);
            return personDevices;
        }

        public GetPersonResponse GetPerson(string xmGuid)
        {
            GetPersonResponse person = null;
            person = _xmatterPersonService.GetPerson(xmGuid);
            return person;
        }

        public GroupMembers GetGroups(string xmGuid, bool returnAll = false)
        {
            GroupMembers groups = null;
            groups = _xmatterPersonService.GetPersonGroups(xmGuid, returnAll);
            return groups;
        }

        public void SavePersonNotificationGroups(List<NotificationGroup> oldGroups, List<NotificationGroup> newGroups)
        {
            List<NotificationGroupSave> changedNotificationGroups = new List<NotificationGroupSave>();
            NotificationGroupSave hasAdded = null;
            foreach (var NewGroupItem in newGroups)
            {
                var OldGroupItem = oldGroups.Find(i => i.EventsGroupName == NewGroupItem.EventsGroupName);

                foreach (var NewDeviceNotificationItem in NewGroupItem.DeviceNotifications)
                {
                    var OldDeviceNotificationItem = OldGroupItem.DeviceNotifications.Find(i => i.DeviceId == NewDeviceNotificationItem.DeviceId);

                    hasAdded = changedNotificationGroups.Where(g => g.GroupName == NewGroupItem.EventsGroupName && g.DeviceId == NewDeviceNotificationItem.DeviceId).FirstOrDefault();
                    if (hasAdded == null)
                        changedNotificationGroups.Add(new NotificationGroupSave()
                        {
                            GroupName = NewGroupItem.EventsGroupName,
                            DeviceId = NewDeviceNotificationItem.DeviceId,
                            DeviceType = NewDeviceNotificationItem.DeviceType,
                            IsSelectedPrevious = OldDeviceNotificationItem == null ? false : OldDeviceNotificationItem.IsEventsSubscribed,
                            IsSelectedCurrent = NewDeviceNotificationItem.IsEventsSubscribed
                        });
                    else if (hasAdded.IsAddDevice || NewDeviceNotificationItem.IsEventsSubscribed)
                        hasAdded.IsSelectedCurrent = true;

                    hasAdded = null;
                    hasAdded = changedNotificationGroups.Where(g => g.GroupName == NewGroupItem.MaintainanceGroupName && g.DeviceId == NewDeviceNotificationItem.DeviceId).FirstOrDefault();
                    if (hasAdded == null)
                        changedNotificationGroups.Add(new NotificationGroupSave()
                        {
                            GroupName = NewGroupItem.MaintainanceGroupName,
                            DeviceId = NewDeviceNotificationItem.DeviceId,
                            DeviceType = NewDeviceNotificationItem.DeviceType,
                            IsSelectedPrevious = OldDeviceNotificationItem == null ? false : OldDeviceNotificationItem.IsMaintainanceSubscribed,
                            IsSelectedCurrent = NewDeviceNotificationItem.IsMaintainanceSubscribed
                        });
                    else if (hasAdded.IsAddDevice || NewDeviceNotificationItem.IsMaintainanceSubscribed)
                        hasAdded.IsSelectedCurrent = true;
                }
            }

            SetGroupIdFromXMatters(changedNotificationGroups.Where(g => g.IsSelectedCurrent != g.IsSelectedPrevious).ToList());

            //foreach(var item in changedNotificationGroups.Where(g => g.IsSelectedCurrent != g.IsSelectedPrevious).ToList())
            Parallel.ForEach(changedNotificationGroups.Where(g => g.IsSelectedCurrent != g.IsSelectedPrevious && !string.IsNullOrWhiteSpace(g.GroupId)).ToList(), item =>
            {
                if (item.IsAddDevice)
                {
                    try
                    {
                        _xmatterGroupService.AddDeviceToGroup(item.GroupId, item.DeviceId);
                        OnNotificationGroupSaved?.Invoke(Constants.AuditType.SUBSCRIBED_NOTIFICATION_GROUP, item);
                    }
                    catch { }
                    try
                    {
                        AddDeviceToShift(item.GroupId, item.DeviceId);
                    }
                    catch { }
                }
                else if (item.IsRemoveDevice)
                {
                    _xmatterGroupService.RemoveDeviceFromGroup(item.GroupId, item.DeviceId);
                    OnNotificationGroupSaved?.Invoke(Constants.AuditType.UNSUBSCRIBED_NOTIFICATION_GROUP, item);
                }
            });
        }
        public void AddDeviceToShift(string groupId, string deviceId)
        {
            string createdShiftId = null;
            string shiftName = ConfigurationManager.AppSettings["24x7Shift"];
            var groupShift = _xmatterGroupService.GetShift(groupId);
            var shifts = groupShift.GroupShift.Find(i => i.Name == shiftName);
            if (shifts == null)
            {
                GroupShift shift = new GroupShift();
                lock (lockShiftCreation)
                {
                    groupShift = _xmatterGroupService.GetShift(groupId);
                    shifts = groupShift.GroupShift.Find(i => i.Name == shiftName);
                    if (shifts == null)
                    {
                        shift = _xmatterGroupService.CreateShift(groupId);
                        createdShiftId = shift.Id;
                    }
                }
            }
            if (!string.IsNullOrEmpty(createdShiftId) || (shifts != null && !string.IsNullOrEmpty(shifts.Id)))
            {
                _xmatterGroupService.AddDeviceToShift(groupId, deviceId);
            }
        }
        public bool UpdatePersonTimeZone(Person person)
        {

            if (person != null)
            {
                Person personToUpdate = new Person
                {
                    Id = person.Id,
                    Timezone = person.Timezone
                };
                _xmatterPersonService.UpdatePerson(personToUpdate);
                OnPersonDataSaved?.Invoke(Constants.AuditType.EDIT_XMATTERS_USERDETAILS, personToUpdate);
                return true;
            }
            return false;
        }

        public List<string> GetTopLevelAccount(string adfsguid)
        {
            return _notificationACRService.GetTopLevelAccount(adfsguid);
        }

        #region PrivateMethods

        private List<DeviceNotifications> GetDevicesForNotificationGrop(PersonDevices devices)
        {
            var deviceNotificationsList = new List<DeviceNotifications>();

            deviceNotificationsList.Add(new DeviceNotifications() { DeviceType = "Secondary Email" });
            deviceNotificationsList.Add(new DeviceNotifications() { DeviceType = "Work Email" });
            deviceNotificationsList.Add(new DeviceNotifications() { DeviceType = "SMS Phone" });

            if (devices != null && devices.Count > 0)
            {
                foreach (var item in deviceNotificationsList)
                {
                    var device = devices.Devices.Find(i => i.Name == item.DeviceType);
                    if (device != null)
                    {
                        item.DeviceId = device.Id;
                    }
                }
            }
            return deviceNotificationsList;
        }

        private bool GetNotificationGroupStatus(GroupMembers groupMembers, string deviceId, string notificationGroupName)
        {
            if (groupMembers == null || string.IsNullOrEmpty(deviceId))
                return false;

            var grp = groupMembers.GroupMember.Find(m => m.Group.TargetName.Equals(notificationGroupName) && m.Member.Id.Equals(deviceId));

            return (grp != null);
        }

        /// <summary>
        /// Sets group Id from XMatters using Group Name, if group does not exists create new one
        /// </summary>
        /// <param name="list"></param>
        private void SetGroupIdFromXMatters(List<NotificationGroupSave> list)
        {
            foreach (var groupName in list.Select(i => i.GroupName).Distinct())
            {
                string groupId = _xmatterGroupService.GetGroupIdFromName(groupName);
                if (string.IsNullOrEmpty(groupId))
                {
                    Group group = null;
                    try
                    {
                        group = _xmatterGroupService.CreateGroup(new Group() { TargetName = groupName });
                    }
                    catch
                    {
                    }
                    if (group != null)
                        groupId = group.Id;
                }

                list.Where(i => i.GroupName == groupName).ToList().ForEach(j => j.GroupId = groupId);
            }
        }
        private bool IsiAIMVer2Enabled
        {
            get
            {
                if (_isiAIMVer2Enabled.HasValue)
                    return _isiAIMVer2Enabled.Value;

                string value = ConfigurationManager.AppSettings["IsiAIMVer2:Enabled"];
                if (!string.IsNullOrEmpty(value))
                    _isiAIMVer2Enabled = Convert.ToBoolean(value);
                else
                    _isiAIMVer2Enabled = false;

                return _isiAIMVer2Enabled.Value;
            }
        }
        #endregion
    }
}
