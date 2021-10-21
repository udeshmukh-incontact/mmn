using ManageMyNotificationsBusinessLayer;
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
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ManageMyNotificationsMVCTests.Controllers
{
    [TestFixture]
    public class GetPersonDevicesTests
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
        public void GetPersonDevices_Throws_Error()
        {
            string xmGuid = "testxmGuid";
            PersonDevices devices = GetPersonDevices();
            GetPersonResponse person = new GetPersonResponse();
            person.Id = "testid";
            person.TargetName = "testid";
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            detail.XMGuid = "";
            List<NotificationGroup> groups = new List<NotificationGroup>();
            _personLogic.Setup(x => x.GetPersonDevices(xmGuid)).Returns(devices);
            _personLogic.Setup(x => x.GetPerson(xmGuid)).Returns(person);
            _personLogic.Setup(x => x.GetSfContact(It.IsAny<List<NotificationContact>>())).Returns(person);
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Throws(new Exception("", new NotFoundException()));
            _personLogic.Setup(x => x.UpdateNotificationProfile(It.IsAny<string>(), It.IsAny<string>()));
            _personLogic.Setup(x => x.GetSFNotificationProfile(It.IsAny<string>())).Returns(detail);
            JsonResult result = _controller.GetPersonDevices();

            Assert.AreEqual("error", result.Data);
            _personLogic.Verify(x => x.GetPersonDevices(xmGuid), Times.Never);
            _personLogic.Verify(x => x.GetPerson(xmGuid), Times.Never);
            _personLogic.Verify(x => x.UpdateNotificationProfile(adfsGuid, xmGuid), Times.Never);
        }

        [Test]
        public void GetPersonDevices_ForExistingUser_Status_Inactive()
        {
            string xmGuid = "testxmGuid";
            PersonDevices devices = null;
            GroupMembers xmGroups = null;
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid",
                Status = "INACTIVE"
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

            _personLogic.Verify(x => x.GetPersonDevices(xmGuid), Times.Once);
            _personLogic.Verify(x => x.GetPerson(xmGuid), Times.Once);
            _personLogic.Verify(x => x.UpdateNotificationProfile(adfsGuid, xmGuid), Times.Never);
        }

        [Test]
        public void GetPersonDevices_ForExistingUser_Status_Inactive_Id_Null()
        {
            string xmGuid = "testxmGuid";
            PersonDevices devices = null;
            GroupMembers xmGroups = null;
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "",
                Status = "INACTIVE"
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

            _personLogic.Verify(x => x.GetPersonDevices(xmGuid), Times.Once);
            _personLogic.Verify(x => x.GetPerson(xmGuid), Times.Once);
            _personLogic.Verify(x => x.UpdateNotificationProfile(adfsGuid, xmGuid), Times.Once);
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

        #region Private Methods
        private void InitController()
        {
            List<Claim> claims = new List<Claim>();
            Claim adfsGuidClaim = new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, Convert.ToBase64String(new Guid(adfsGuid).ToByteArray()));
            claims.Add(adfsGuidClaim);
            Claim userNameClaim = new Claim(System.Security.Claims.ClaimTypes.Name, "TestUserName");
            claims.Add(userNameClaim);

            NotificationManagerViewModel sessionModel = new NotificationManagerViewModel();
            sessionModel.Person = new Persons();
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
