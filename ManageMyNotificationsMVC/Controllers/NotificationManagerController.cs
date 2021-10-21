using ManageMyNotificationsBusinessLayer;
using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Interfaces;
using ManageMyNotificationsMVC.Controllers.Converters;
using ManageMyNotificationsMVC.Models;
using Microsoft.IdentityModel.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ManageMyNotificationsMVC.Controllers
{
    public class NotificationManagerController : BaseController
    {
        private readonly IPersonBusinessLogic _PersonLogic;
        private readonly IAuditLogService _auditLogService;
        protected const string UserSession = "AdfsUserEmail";
        private Dictionary<string, List<string>> _subscribedData;
        private Dictionary<string, List<string>> _unSubscribedData;
        private bool? _isAuditLogEnabled = null;

        public NotificationManagerController(IPersonBusinessLogic personLogic, IAuditLogService auditLogService)
        {
            _PersonLogic = personLogic;
            _auditLogService = auditLogService;
            _PersonLogic.OnPersonDataSaved += OnPersonDataSaved;
            _PersonLogic.OnNotificationGroupSaved += OnNotificationGroupSaved;
            _PersonLogic.OnDeviceSaved += OnDeviceSaved;
        }

        public JsonResult GetPersonDevices()
        {
            var viewModel = new NotificationManagerViewModel();

            GetPersonResponse person = null;
            PersonDevices devices = null;
            GroupMembers groups = null;
            string xmpartnerprefix = string.Empty;


            string userAdfsGuid = User.GetAdfsGuid();

            var userProfile = _PersonLogic.GetSFNotificationProfile(userAdfsGuid);

            if (!string.IsNullOrWhiteSpace(userProfile.XMGuid))
            {
                Parallel.Invoke(
                    () => devices = _PersonLogic.GetPersonDevices(userProfile.XMGuid),
                    () => person = _PersonLogic.GetPerson(userProfile.XMGuid),
                    () => groups = _PersonLogic.GetGroups(userProfile.XMGuid, true)
                );
            }

            if (person == null)
            {
                person = _PersonLogic.GetSfContact(userProfile.NotificationContacts);

                try
                {
                    var existingPerson = GetPersonByTargetName(person.TargetName);
                    userProfile.NotificationContacts.First().XMPersonId = existingPerson.TargetName;
                    _PersonLogic.SyncSFXMattersProfile(userProfile.NotificationContacts, existingPerson);
                    userProfile.XMGuid = existingPerson.Id;
                    Parallel.Invoke(
                        () => devices = _PersonLogic.GetPersonDevices(userProfile.XMGuid),
                        () => person = _PersonLogic.GetPerson(userProfile.XMGuid),
                        () => groups = _PersonLogic.GetGroups(userProfile.XMGuid, true)
                    );
                }
                catch (Exception ex) when (ex.InnerException is NotFoundException) { }
                //catch (Exception ex)
                //{
                //    if (!(ex.InnerException is NotFoundException))
                //        throw ex;
                //}
            }
            else
            {
                _PersonLogic.SyncSFXMattersProfile(userProfile.NotificationContacts, person);
                if (person.Id.ToString() != userProfile.XMGuid)
                    _PersonLogic.UpdateNotificationProfile(userAdfsGuid, userProfile.XMGuid);
                if (!String.IsNullOrEmpty(person.Id) && person.Status == "INACTIVE")
                {
                    viewModel.NotificationGroups = new List<NotificationGroup>();
                    viewModel.Person = person.ToViewModel();
                    return Json(viewModel, JsonRequestBehavior.AllowGet);
                }
            }

            if (devices != null)
                viewModel = devices.ToViewModel();

            if (person != null)
            {
                viewModel.NotificationGroups = _PersonLogic.GetPersonNotificationGroups(person, devices, groups, userProfile.Events_Notifications, userProfile.Maintainance_Notifications, userProfile.NotificationContacts);
                viewModel.Person = person.ToViewModel();
                viewModel.Person.Language = userProfile?.LanguagePreference;
            }

            if (person == null || viewModel.NotificationGroups == null || viewModel.NotificationGroups.Count() == 0)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }

            Session["NotificationManager"] = viewModel;

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(true)]
        public JsonResult SavenotificationProfile(NotificationManagerViewModel notification)
        {
            string userAdfsGuid = User.GetAdfsGuid();
            if (IsImpersonationModeOn())
                return Json("");
            try
            {
                _PersonLogic.UpdateSFContactsFirstnameLastName(notification.Person.FirstName, notification.Person.LastName, userAdfsGuid);
            }
            catch (Exception ex)
            {

            }
            return Json("success");
        }

        [HttpPost]
        [ValidateInput(true)]
        public JsonResult SaveNotification(NotificationManagerViewModel notification)
        {
            var currentDeviceType = "";
            bool updateTimeframe = false;
            Person person;
            GetPersonResponse existingPerson = null;
            if (IsImpersonationModeOn())
                return Json("");

            try
            {
                _subscribedData = new Dictionary<string, List<string>>();
                _unSubscribedData = new Dictionary<string, List<string>>();

                string userAdfsGuid = User.GetAdfsGuid();
                PersonDevices sessionNotification = new PersonDevices();
                if (Session["NotificationManager"] == null)
                {
                    GetPersonDevices();
                }
                sessionNotification = (Session["NotificationManager"] as NotificationManagerViewModel).ToDomainModel();

                if (sessionNotification.Person.Id == null)
                {
                    try
                    {
                        existingPerson = GetPersonByTargetName(sessionNotification.Person.TargetName);
                        sessionNotification.Person.Id = new Guid(existingPerson.Id);
                    }
                    catch (Exception ex) when (ex.InnerException is NotFoundException) { }
                    person = CreateXMPerson(sessionNotification.Person, notification.Person.TimeZone);
                    if (!string.IsNullOrEmpty(person.Id.ToString()))
                    {
                        notification.Person.Id = person.Id.ToString();
                    }
                }

                var clientnotification = notification.ToDomainModel();

                if (sessionNotification != null && sessionNotification.Person.Timezone != clientnotification.Person.Timezone)
                {
                    updateTimeframe = true;
                    _PersonLogic.UpdatePersonTimeZone(clientnotification.Person);
                }

                if (sessionNotification.Devices == null || sessionNotification.Devices.Count == 0)
                {
                    foreach (var item in clientnotification.Devices)
                    {
                        item.Status = "new";
                    }
                }
                else
                {
                    foreach (var item in clientnotification.Devices.Where(d => string.IsNullOrWhiteSpace(d.Id)))
                    {
                        item.Status = "new";
                    }
                    foreach (var item in sessionNotification.Devices)
                    {
                        var device = clientnotification.Devices.FirstOrDefault(d => d.Id == item.Id);
                        if (device == null)
                        {
                            item.Status = "delete";
                            clientnotification.Devices.Add(item);
                        }
                        else if (item.Name != device.Name)
                        {
                            item.Status = "delete";
                            clientnotification.Devices.Add(item);
                            device.Id = null;
                            device.Status = "new";
                        }
                        else if (item.IsEquals(device))
                        {
                            device.Status = "nochange";
                        }
                        else
                        {
                            device.Status = "update";
                        }
                    }
                }

                Dictionary<string, string> keyValueDevices = new Dictionary<string, string>();

                if (clientnotification != null)
                {
                    foreach (var item in clientnotification.Devices.OrderBy(o => o.Status))
                    {
                        DeviceResponse response = new DeviceResponse();
                        DeviceRequest deviceToAdd = new DeviceRequest();
                        deviceToAdd.Description = item.Description;
                        deviceToAdd.Id = item.Id;
                        deviceToAdd.DeviceType = item.DeviceType;
                        deviceToAdd.Name = item.Name;
                        deviceToAdd.Owner = notification.Person.Id;
                        deviceToAdd.EmailAddress = item.EmailAddress;
                        deviceToAdd.PhoneNumber = item.PhoneNumber;
                        deviceToAdd.Country = item.Country;
                        deviceToAdd.Timeframes = item.TimeFrames.data;
                        currentDeviceType = item.Name;
                        if (item.Status == "new" && string.IsNullOrEmpty(item.Id))
                            response = _PersonLogic.CreateDevice(deviceToAdd);
                        else if (item.Status == "update" || (item.Name == "SMS Phone" && updateTimeframe && item.Status == "nochange"))
                        {
                            response = _PersonLogic.CreateDevice(deviceToAdd);
                            if (item.Name == "SMS Phone")
                                CheckTimeframesAndAddToAuditLog(item.TimeFrames.data);
                        }
                        else if (item.Status == "delete")
                            response = _PersonLogic.RemoveDevice(deviceToAdd);

                        if (response != null && !string.IsNullOrWhiteSpace(response.Id))
                            item.Id = response.Id;

                        if (item.Status != "delete")
                        {
                            keyValueDevices.Add(item.Name, item.Id);
                        }
                    }
                }

                clientnotification.NotificationGroups.ForEach(item =>
                {
                    List<DeviceNotifications> removeNG = new List<DeviceNotifications>();
                    item.DeviceNotifications.ForEach(n =>
                    {
                        string dValue = "";
                        if (keyValueDevices.TryGetValue(n.DeviceType, out dValue))
                        {
                            n.DeviceId = keyValueDevices[n.DeviceType];
                        }
                        else
                        {
                            removeNG.Add(n);
                        }
                    });
                    removeNG.ForEach(r => item.DeviceNotifications.Remove(r));
                });

                _PersonLogic.SavePersonNotificationGroups(sessionNotification.NotificationGroups, clientnotification.NotificationGroups);
                return Json("success");
            }
            catch (Exception fex) when (fex.InnerException is FormatException)
            {
                if (fex.Message.ToLower().Contains("phone"))
                {
                    GetPersonDevices();
                    return Json("invalidPhoneNumber");
                }
                else if (fex.Message.ToLower().Contains("email") && currentDeviceType == "Work Email")
                {
                    GetPersonDevices();
                    return Json("invalidPrimaryEmail");
                }
                else if (fex.Message.ToLower().Contains("email") && currentDeviceType == "Secondary Email")
                {
                    GetPersonDevices();
                    return Json("invalidSecondaryEmail");
                }
                else
                    throw new Exception(fex.Message);

            }

            finally
            {
                SaveAuditLogs(notification.Person.Id);
            }
        }

        [HttpPost]
        [ValidateInput(true)]
        public JsonResult ChangeNotificationProfileStatus(Persons person)
        {
            string userAdfsGuid = User.GetAdfsGuid();
            if (IsImpersonationModeOn())
                return Json("");
            try
            {
                if (person.Status == "ACTIVE")
                    _PersonLogic.EnableXMattersProfile(person.Id.ToString());
                else
                    _PersonLogic.DisableXMattersProfile(userAdfsGuid, person.Id.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SaveAuditLogs(person.Id);
            }

            return Json("success");
        }

        #region Private Method
        private Person CreateXMPerson(Person sessionPerson, string timeZone)
        {
            List<string> ObjRoles = new List<string>();
            ObjRoles.Add("No Access User");
            sessionPerson.roles = ObjRoles;
            sessionPerson.Status = "Active";
            sessionPerson.Timezone = timeZone;
            var person = _PersonLogic.CreateXmattersPerson(User.GetAdfsGuid(), sessionPerson);
            return person;
        }

        private GetPersonResponse GetPersonByTargetName(string targetName)
        {
            return _PersonLogic.GetPersonByTargetName(targetName);
        }

        public ActionResult logout(bool timeout = false)
        {
            string absoluteUrl = Request.Url.AbsoluteUri;
            string replyUrl = string.Empty;
            if (timeout)
                replyUrl = Url.Action("Index", "SessionTimeOut", new { user = User.GetAdfsGuid() }, Request.Url.Scheme);
            else
                replyUrl = absoluteUrl.Substring(0, absoluteUrl.LastIndexOf("/") + 1);
            WSFederationAuthenticationModule.FederatedSignOut(new Uri(FederatedAuthentication.WSFederationAuthenticationModule.Issuer), new Uri(replyUrl));
            return null;
        }

        [ExcludeFromCodeCoverage]
        private void OnDeviceSaved(string type, DeviceRequest device)
        {
            try
            {
                if (!IsAuditLogEnabled)
                    return;
                string description = string.Empty;
                string deviceValue = device.Name == "SMS Phone" ? device.PhoneNumber : device.EmailAddress;
                if (type == Constants.AuditType.ADDED_NOTIFICATION_DEVICE)
                    description = string.Format(Constants.AuditLogMessage.MSG_ADDED_DEVICE, device.Name, deviceValue);
                else if (type == Constants.AuditType.UPDATED_NOTIFICATION_DEVICE)
                {
                    var notifications = Session["NotificationManager"] as NotificationManagerViewModel;
                    var oldDevice = notifications.Devices.Find(x => x.Id == device.Id);
                    string fromValue = (oldDevice != null && oldDevice.Name == "SMS Phone" ? oldDevice.PhoneNumber : oldDevice?.EmailAddress);
                    if (fromValue != deviceValue)
                        description = string.Format(Constants.AuditLogMessage.MSG_UPDATED_DEVICE, device.Name, fromValue, deviceValue);
                }
                else if (type == Constants.AuditType.REMOVED_NOTIFICATION_DEVICE)
                    description = string.Format(Constants.AuditLogMessage.MSG_REMOVED_DEVICE, device.Name, deviceValue);
                if (!string.IsNullOrEmpty(description))
                    AddAuditLog(type, description);
            }
            catch (Exception ex)
            {
                _elmahWrapper.Raise(ex);
            }
        }

        [ExcludeFromCodeCoverage]
        private void OnNotificationGroupSaved(string type, NotificationGroupSave notificationGroup)
        {
            try
            {
                if (!IsAuditLogEnabled)
                    return;
                string description = string.Empty;
                if (type == Constants.AuditType.SUBSCRIBED_NOTIFICATION_GROUP)
                    AddToGroupsDictionary(_subscribedData, notificationGroup);

                if (type == Constants.AuditType.UNSUBSCRIBED_NOTIFICATION_GROUP)
                    AddToGroupsDictionary(_unSubscribedData, notificationGroup);
            }
            catch (Exception ex)
            {
                _elmahWrapper.Raise(ex);
            }
        }

        [ExcludeFromCodeCoverage]
        private void AddToGroupsDictionary(Dictionary<string, List<string>> dictionary, NotificationGroupSave notificationGroup)
        {
            if (dictionary.ContainsKey(notificationGroup.DeviceType))
                dictionary[notificationGroup.DeviceType].Add(notificationGroup.GroupName);
            else
                dictionary[notificationGroup.DeviceType] = new List<string>() { notificationGroup.GroupName };
        }

        [ExcludeFromCodeCoverage]
        private void OnPersonDataSaved(string type, Person person)
        {
            try
            {
                if (!IsAuditLogEnabled)
                    return;
                string description = string.Empty;
                var sessionNotification = (Session["NotificationManager"] as NotificationManagerViewModel);
                if (type == Constants.AuditType.ENABLED_NOTIFICATION_PROFILE)
                    description = Constants.AuditLogMessage.MSG_PROFILE_ENABLED;
                else if (type == Constants.AuditType.DISABLED_NOTIFICATION_PROFILE)
                    description = Constants.AuditLogMessage.MSG_PROFILE_DISABLED;
                else if (type == Constants.AuditType.EDIT_XMATTERS_USERDETAILS)
                {
                    if (sessionNotification != null && sessionNotification.Person != null && sessionNotification.Person.Id != null)
                        description = string.Format(Constants.AuditLogMessage.MSG_EDIT_USERDETAILS, "TimeZone", sessionNotification.Person.TimeZone == null ? "''" : sessionNotification.Person.TimeZone, person?.Timezone);
                }

                AddAuditLog(type, description);
            }
            catch (Exception ex)
            {
                _elmahWrapper.Raise(ex);
            }
        }

        [ExcludeFromCodeCoverage]
        private void CheckTimeframesAndAddToAuditLog(List<TimeFramedetails> data)
        {
            try
            {
                if (!IsAuditLogEnabled)
                    return;
                bool isTimeframeChanged = false;
                var sessionNotification = (Session["NotificationManager"] as NotificationManagerViewModel);
                Device device = sessionNotification?.Devices.Find(x => x.Name == "SMS Phone");
                if (device == null)
                    return;
                if (device != null && device.DeviceTimeFrame.Count != data.Count)
                    isTimeframeChanged = true;
                else
                {
                    foreach (TimeFrame tf in device.DeviceTimeFrame)
                    {
                        if (!data.Any(x => x.name == tf.Name))
                        {
                            isTimeframeChanged = true;
                            break;
                        }
                        else
                        {
                            var d = data.Find(x => x.name == tf.Name);
                            if (d.durationInMinutes != tf.DurationInMinutes)
                            {
                                isTimeframeChanged = true;
                                break;
                            }
                            else if (d.days.Count != tf.Days.Count)
                            {
                                isTimeframeChanged = true;
                                break;
                            }
                            else
                            {
                                foreach (string day in tf.Days)
                                {
                                    if (!d.days.Any(x => x == day))
                                    {
                                        isTimeframeChanged = true;
                                        break;
                                    }
                                }
                            }

                        }
                    }
                }
                if (isTimeframeChanged)
                    AddAuditLog(Constants.AuditType.DEVICE_TIMEFRAME_CHANGED, Constants.AuditLogMessage.MSG_TIMEFRAMECHANGED);
            }
            catch (Exception ex)
            {
                _elmahWrapper.Raise(ex);
            }
        }

        [ExcludeFromCodeCoverage]
        private void AddAuditLog(string type, string description)
        {
            try
            {
                if (!IsAuditLogEnabled || string.IsNullOrEmpty(description))
                    return;
                _auditLogService.AddToAuditLogMessageCollection(type, description);
            }
            catch (Exception ex)
            {
                _elmahWrapper.Raise(ex);
            }
        }

        [ExcludeFromCodeCoverage]
        private void SaveAuditLogs(string xmGuid)
        {
            try
            {
                if (!IsAuditLogEnabled)
                    return;
                AddNotificationMessagesToAuditLog();
                _auditLogService.SaveAuditLogs(User.GetAdfsGuid(), User.GetUserName(), xmGuid);
            }
            catch (Exception ex)
            {
                _elmahWrapper.Raise(ex);
            }
        }

        [ExcludeFromCodeCoverage]
        private void AddNotificationMessagesToAuditLog()
        {
            AddMessagesFromDictionary(_subscribedData, Constants.AuditType.SUBSCRIBED_NOTIFICATION_GROUP);
            AddMessagesFromDictionary(_unSubscribedData, Constants.AuditType.UNSUBSCRIBED_NOTIFICATION_GROUP);
        }

        [ExcludeFromCodeCoverage]
        private void AddMessagesFromDictionary(Dictionary<string, List<string>> dictionary, string type)
        {
            if (dictionary == null)
                return;
            foreach (KeyValuePair<string, List<string>> data in dictionary)
            {
                string description = string.Empty;
                string action = (type == Constants.AuditType.SUBSCRIBED_NOTIFICATION_GROUP ? "subscribed" : "unsubscribed");
                description = string.Format(Constants.AuditLogMessage.MSG_SUBSCRIBED_UNSUBSCRIBED, data.Key, action, String.Join(",", data.Value));
                AddAuditLog(type, description);
            }
        }

        [ExcludeFromCodeCoverage]
        private bool IsAuditLogEnabled
        {
            get
            {
                if (_isAuditLogEnabled.HasValue)
                    return _isAuditLogEnabled.Value;

                string value = ConfigurationManager.AppSettings["EnableXMattersAuditLog"];
                if (!string.IsNullOrEmpty(value))
                    _isAuditLogEnabled = Convert.ToBoolean(value);
                else
                    _isAuditLogEnabled = false;

                return _isAuditLogEnabled.Value;
            }
        }
        #endregion
    }
}