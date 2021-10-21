using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Data.Salesforce;
using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using ManageMyNotificationsBusinessLayer.Interfaces;
using ManageMyNotificationsMVC.Controllers;
using ManageMyNotificationsMVC.Models;
using Microsoft.IdentityModel.Claims;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ManageMyNotificationsMVCTests.Controllers
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class NotificationManagerControllerTests
    {
        private NotificationManagerController _controller;
        private Mock<IPersonBusinessLogic> _personLogic;
        private Mock<IAuditLogService> _auditLogService;
        private string adfsGuid = "c18a5aa0-d576-447d-a563-44dcba28a509";

        [SetUp]
        public void Setup()
        {
            _personLogic = new Mock<IPersonBusinessLogic>();
            _auditLogService = new Mock<IAuditLogService>();
            InitController();
        }

        [Test]
        public void GetPersonDevices_ReturnsError_ForExistingUser()
        {
            string xmGuid = "testxmGuid";
            PersonDevices devices = null;
            GroupMembers xmGroups = null;
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid"
            };
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            List<NotificationGroup> groups = null;

            _personLogic.Setup(x => x.GetSFNotificationProfile(adfsGuid)).Returns(detail);
            _personLogic.Setup(x => x.GetPersonDevices(xmGuid)).Returns(devices);
            _personLogic.Setup(x => x.GetPerson(xmGuid)).Returns(person);
            _personLogic.Setup(x => x.GetGroups(It.IsAny<string>(), It.IsAny<bool>())).Returns(xmGroups);
            _personLogic.Setup(x => x.GetSfContact(It.IsAny<List<NotificationContact>>())).Returns(person);
            _personLogic.Setup(x => x.SyncSFXMattersProfile(It.IsAny<List<NotificationContact>>(), It.IsAny<GetPersonResponse>())).Returns(true);
            _personLogic.Setup(x => x.UpdateNotificationProfile(adfsGuid, xmGuid));
            _personLogic.Setup(x => x.GetPersonNotificationGroups(person, devices, xmGroups, detail.Events_Notifications, detail.Maintainance_Notifications, detail.NotificationContacts)).Returns(groups);

            JsonResult result = _controller.GetPersonDevices();

            Assert.AreEqual("error", result.Data);
            _personLogic.Verify(x => x.GetPersonDevices(xmGuid), Times.Once);
            _personLogic.Verify(x => x.GetPerson(xmGuid), Times.Once);
            _personLogic.Verify(x => x.UpdateNotificationProfile(adfsGuid, xmGuid), Times.Never);
        }

        [Test]
        public void GetPersonDevices_ReturnsError_ForNewUser()
        {
            string xmGuid = "testxmGuid";
            PersonDevices devices = GetPersonDevices();
            GetPersonResponse person = new GetPersonResponse();
            GroupMembers xmGroups = new GroupMembers();
            person.Id = xmGuid;
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            List<NotificationGroup> groups = new List<NotificationGroup>();
            _personLogic.Setup(x => x.GetSFNotificationProfile(adfsGuid)).Returns(detail);
            _personLogic.Setup(x => x.GetPersonDevices(xmGuid)).Returns(devices);
            _personLogic.Setup(x => x.GetPerson(xmGuid)).Returns(person);
            _personLogic.Setup(x => x.GetGroups(It.IsAny<string>(), It.IsAny<bool>())).Returns(xmGroups);
            _personLogic.Setup(x => x.GetSfContact(It.IsAny<List<NotificationContact>>())).Returns(person);
            _personLogic.Setup(x => x.SyncSFXMattersProfile(It.IsAny<List<NotificationContact>>(), It.IsAny<GetPersonResponse>())).Returns(true);
            _personLogic.Setup(x => x.UpdateNotificationProfile(adfsGuid, xmGuid));
            _personLogic.Setup(x => x.GetPersonNotificationGroups(person, devices, xmGroups, detail.Events_Notifications, detail.Maintainance_Notifications, detail.NotificationContacts)).Returns(groups);
            JsonResult result = _controller.GetPersonDevices();

            Assert.AreEqual("error", result.Data);
            _personLogic.Verify(x => x.GetPersonDevices(xmGuid), Times.Once);
            _personLogic.Verify(x => x.GetPerson(xmGuid), Times.Once);
            _personLogic.Verify(x => x.UpdateNotificationProfile(adfsGuid, xmGuid), Times.Never);
        }

        [Test]
        public void GetPersonDevices_ReturnsError_ForExistingUser_WithDifferent_PersonIdAndXmGuid()
        {
            string xmGuid = "testxmGuid";
            PersonDevices devices = GetPersonDevices();
            GetPersonResponse person = new GetPersonResponse();
            person.Id = "testid";
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            List<NotificationGroup> groups = new List<NotificationGroup>();
            _personLogic.Setup(x => x.GetPersonDevices(xmGuid)).Returns(devices);
            _personLogic.Setup(x => x.GetPerson(xmGuid)).Returns(person);
            _personLogic.Setup(x => x.UpdateNotificationProfile(adfsGuid, xmGuid));
            _personLogic.Setup(x => x.GetSFNotificationProfile(adfsGuid)).Returns(detail);
            JsonResult result = _controller.GetPersonDevices();

            Assert.AreEqual("error", result.Data);
            _personLogic.Verify(x => x.GetPersonDevices(xmGuid), Times.Once);
            _personLogic.Verify(x => x.GetPerson(xmGuid), Times.Once);
            _personLogic.Verify(x => x.UpdateNotificationProfile(adfsGuid, xmGuid), Times.Once);
        }

        [Test]
        public void GetPersonDevices_RunsSuccessfully_ForExistingUser()
        {
            string xmGuid = "testxmGuid";
            PersonDevices devices = GetPersonDevices();
            GetPersonResponse person = new GetPersonResponse();
            GroupMembers xmGroups = null;
            person.Id = xmGuid;
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            List<NotificationGroup> groups = GetNotificationGroups();
            _personLogic.Setup(x => x.GetPersonDevices(xmGuid)).Returns(devices);
            _personLogic.Setup(x => x.GetPerson(xmGuid)).Returns(person);
            _personLogic.Setup(x => x.UpdateNotificationProfile(adfsGuid, xmGuid));
            _personLogic.Setup(x => x.GetSFNotificationProfile(adfsGuid)).Returns(detail);
            _personLogic.Setup(x => x.GetPersonNotificationGroups(person, devices, xmGroups, detail.Events_Notifications, detail.Maintainance_Notifications, detail.NotificationContacts)).Returns(groups);
            JsonResult result = _controller.GetPersonDevices();
            NotificationManagerViewModel model = result.Data as NotificationManagerViewModel;
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Devices);
            Assert.IsNotNull(model.Person);
            Assert.IsNotNull(model.NotificationGroups);
            Assert.AreEqual(1, model.Devices.Count);
            Assert.AreEqual(1, model.NotificationGroups.Count);
            Assert.AreEqual(xmGuid, model.Person.Id);
        }

        [Test]
        public void GetPersonDevices_RunsSuccessfully_ForNewUser()
        {
            string xmGuid = "testxmGuid";
            PersonDevices devices = null;
            GroupMembers xmGroups = null;
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "",
                FirstName = "test",
                LastName = "test",
                TargetName = "test"
            };

            GetPersonResponse existingPerson = new GetPersonResponse()
            {
                Id = "testxmGuid",
                FirstName = "test",
                LastName = "test",
                TargetName = "test"
            };
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            detail.XMGuid = "";
            List<NotificationGroup> groups = GetNotificationGroups();

            _personLogic.Setup(x => x.GetSFNotificationProfile(adfsGuid)).Returns(detail);
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Returns(existingPerson);
            _personLogic.Setup(x => x.GetPersonDevices(xmGuid)).Returns(devices);
            _personLogic.Setup(x => x.GetPerson(xmGuid)).Returns(person);
            _personLogic.Setup(x => x.GetGroups(It.IsAny<string>(), It.IsAny<bool>())).Returns(xmGroups);
            _personLogic.Setup(x => x.GetSfContact(It.IsAny<List<NotificationContact>>())).Returns(person);
            _personLogic.Setup(x => x.SyncSFXMattersProfile(It.IsAny<List<NotificationContact>>(), It.IsAny<GetPersonResponse>())).Returns(true);
            _personLogic.Setup(x => x.UpdateNotificationProfile(adfsGuid, xmGuid));
            _personLogic.Setup(x => x.GetPersonNotificationGroups(It.IsAny<GetPersonResponse>(), It.IsAny<PersonDevices>(), It.IsAny<GroupMembers>(), It.IsAny<Dictionary<string, XMUserDetails>>(), It.IsAny<Dictionary<string, XMUserDetails>>(), It.IsAny<List<NotificationContact>>())).Returns(groups);
            JsonResult result = _controller.GetPersonDevices();
            NotificationManagerViewModel model = result.Data as NotificationManagerViewModel;
            Assert.IsNotNull(model);
        }

        [Test]
        public void SavenotificationProfile_ChangeFirstName()
        {
            var notification = new NotificationManagerViewModel()
            {
                Person = new Persons()
                {
                    FirstName = "test",
                    LastName = "test"
                }
            };
            _personLogic.Setup(x => x.UpdateSFContactsFirstnameLastName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            JsonResult result = _controller.SavenotificationProfile(notification);
            Assert.AreEqual("success", result.Data);
        }

        [Test]
        public void SavenotificationProfile_ChangeFirstName_Silent_Catch()
        {
            var notification = new NotificationManagerViewModel()
            {
                Person = new Persons()
                {
                    FirstName = "test",
                    LastName = "test"
                }
            };
            _personLogic.Setup(x => x.UpdateSFContactsFirstnameLastName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            JsonResult result = _controller.SavenotificationProfile(notification);
            Assert.AreEqual("success", result.Data);
        }

        [Test]
        public void ChangeNotificationProfileStatus_Enable_Success()
        {
            var person = new Persons()
            {
                Id = "test",
                Status = "ACTIVE"
            };
            _personLogic.Setup(x => x.EnableXMattersProfile(It.IsAny<string>()));
            _personLogic.Setup(x => x.DisableXMattersProfile(It.IsAny<string>(), It.IsAny<string>()));
            JsonResult result = _controller.ChangeNotificationProfileStatus(person);
            Assert.AreEqual("success", result.Data);
            _personLogic.Verify(x => x.EnableXMattersProfile(It.IsAny<string>()), Times.Once);
            _personLogic.Verify(x => x.DisableXMattersProfile(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ChangeNotificationProfileStatus_Disable_Success()
        {
            var person = new Persons()
            {
                Id = "test",
                Status = "NotActive"
            };
            _personLogic.Setup(x => x.EnableXMattersProfile(It.IsAny<string>()));
            _personLogic.Setup(x => x.DisableXMattersProfile(It.IsAny<string>(), It.IsAny<string>()));
            JsonResult result = _controller.ChangeNotificationProfileStatus(person);
            Assert.AreEqual("success", result.Data);
            _personLogic.Verify(x => x.EnableXMattersProfile(It.IsAny<string>()), Times.Never);
            _personLogic.Verify(x => x.DisableXMattersProfile(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ChangeNotificationProfileStatus_Throw_Exception()
        {
            var person = new Persons()
            {
                Id = "test",
                Status = "ACTIVE"
            };
            _personLogic.Setup(x => x.EnableXMattersProfile(It.IsAny<string>())).Throws(new Exception());
            _personLogic.Setup(x => x.DisableXMattersProfile(It.IsAny<string>(), It.IsAny<string>()));
            Assert.Throws<Exception>(()=> _controller.ChangeNotificationProfileStatus(person));
        }

        //[Test]
        //public void SaveNotification_Successfull_ForNewUser_WithoutDevices()
        //{
        //    Person person = new Person();
        //    Guid id = Guid.NewGuid();
        //    person.Id = id;
        //    NotificationManagerViewModel sessionModel = new NotificationManagerViewModel();
        //    sessionModel.Person = new Persons();
        //    sessionModel.Person.TimeZone = "TestTZ";
        //    sessionModel.NotificationGroups = new List<NotificationGroup>();
        //    sessionModel.Devices = new List<Device>();

        //    NotificationManagerViewModel notification = new NotificationManagerViewModel();
        //    notification.Person = new Persons();
        //    notification.Person.TimeZone = "TestTZ_1";
        //    notification.NotificationGroups = new List<NotificationGroup>();
        //    notification.NotificationGroups.Add(new NotificationGroup() { DeviceNotifications = new List<DeviceNotifications>() { new DeviceNotifications() { DeviceType = "Primary Email" }, new DeviceNotifications() { DeviceType = "Secondary Email" } } });
        //    notification.Devices = new List<Device>();
        //    notification.Devices.Add(new Device() { Name = "Primary Email" });

        //    //_controller.ControllerContext.HttpContext.Session["NotificationManager"] = sessionModel;

        //    DeviceResponse dResponse = new DeviceResponse();
        //    dResponse.Id = "deviceId";

        //    _personLogic.Setup(x => x.CreateXmattersPerson(adfsGuid, person)).Returns(person);
        //    _personLogic.Setup(x => x.DisableXMattersProfile(adfsGuid, id.ToString()));
        //    _personLogic.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Returns(dResponse);
        //    _personLogic.Setup(x => x.SavePersonNotificationGroups(sessionModel.NotificationGroups, notification.NotificationGroups));

        //    JsonResult result = _controller.SaveNotification(notification);
        //}

        #region Private Methods
        private void InitController()
        {
            List<Claim> claims = new List<Claim>();
            Claim adfsGuidClaim = new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, Convert.ToBase64String(new Guid(adfsGuid).ToByteArray()));
            claims.Add(adfsGuidClaim);
            Claim userNameClaim = new Claim(System.Security.Claims.ClaimTypes.Name, "TestUserName");
            claims.Add(userNameClaim);

            NotificationManagerViewModel sessionModel = new NotificationManagerViewModel
            {
                Person = new Persons()
            };
            sessionModel.Person.TimeZone = "TestTZ";
            sessionModel.NotificationGroups = new List<NotificationGroup>();
            sessionModel.Devices = new List<Device>();

            var session = new Mock<HttpSessionStateBase>();
            //session.Object["NotificationManager"] = sessionModel;
            var httpContext = new Moq.Mock<HttpContextBase>();
            httpContext.Setup(x => x.User).Returns(new CustomPrincipal(claims));
            httpContext.Setup(x => x.Session).Returns(session.Object);
            httpContext.Setup(x => x.Session["NotificationManager"]).Returns(sessionModel);
            var reqContext = new RequestContext(httpContext.Object, new RouteData());
            _controller = new NotificationManagerController(_personLogic.Object, null);
            _controller.ControllerContext = new ControllerContext(reqContext, _controller);
        }

        private static UserNotificationDetail GetUserNotificationDetail(string xmGuid)
        {
            return new UserNotificationDetail
            {
                NotificationContacts = new List<NotificationContact>()
                {
                    new NotificationContact()
                    {
                        AccountId = "",
                        AccountNumber = "",
                        AccountRootParentIDs = new string[]{""},
                        AdfsGuid = "",
                        ContactRole = "",
                        Deactivated = false,
                        FirstName = "",
                        LastName = "",
                        XMPersonGuid = xmGuid
                    }
                },
                XMGuid = xmGuid
            };
        }

        
        private PersonDevices GetPersonDevices()
        {
            PersonDevices devices = new PersonDevices();
            devices.Devices = new List<DeviceResponse>();
            devices.Devices.Add(new DeviceResponse() { DeviceType = "Primary Email" });
            return devices;
        }

        private List<NotificationGroup> GetNotificationGroups()
        {
            List<NotificationGroup> groups = new List<NotificationGroup>();
            groups.Add(new NotificationGroup() { EventsGroupName = "TestGroup" });
            return groups;
        }
        #endregion
    }
}
