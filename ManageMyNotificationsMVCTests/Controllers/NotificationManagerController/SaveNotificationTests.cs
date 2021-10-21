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
    public class SaveNotificationTests
    {
        private NotificationManagerController _controller;
        private Mock<IPersonBusinessLogic> _personLogic;
        private Mock<IAuditLogService> _auditLogService;
        private readonly string adfsGuid = "c18a5aa0-d576-447d-a563-44dcba28a509";
        private readonly string xmGuid = Guid.NewGuid().ToString();
        private readonly string primaryDeviceId = Guid.NewGuid().ToString();
        private readonly string secondaryDeviceId = Guid.NewGuid().ToString();
        private readonly string smsDeviceId = Guid.NewGuid().ToString();
        private readonly string group1Id = Guid.NewGuid().ToString();
        private readonly string group2Id = Guid.NewGuid().ToString();

        [SetUp]
        public void Setup()
        {
            _personLogic = new Mock<IPersonBusinessLogic>();
            _auditLogService = new Mock<IAuditLogService>();

        }

        [Test]
        public void SaveNotificationWithoutAnyChange()
        {
            InitController();
            string xmGuid = "testxmGuid";
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid"
            };
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);

            _personLogic.Setup(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>())).Returns(GetFakexmPerson());
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Returns(GetFakePerson());
            _personLogic.Setup(x => x.UpdatePersonTimeZone(It.IsAny<Person>())).Returns(true);
            _personLogic.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Returns(GetFakePrimaryDeviceResponse());
            _personLogic.Setup(x => x.RemoveDevice(It.IsAny<DeviceRequest>())).Returns(GetFakePrimaryDeviceResponse());
            _personLogic.Setup(x => x.SavePersonNotificationGroups(It.IsAny<List<NotificationGroup>>(), It.IsAny<List<NotificationGroup>>()));
            JsonResult result = _controller.SaveNotification(GetFakeViewModel());
            Assert.AreEqual("success", result.Data);
            _personLogic.Verify(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>()), Times.Once);
            _personLogic.Verify(x => x.GetPersonByTargetName(It.IsAny<string>()), Times.Once);
            _personLogic.Verify(x => x.CreateDevice(It.IsAny<DeviceRequest>()), Times.Never);
        }

        [Test]
        public void SaveNotification_Throw_Exception_UserFound_WithoutAnyChange()
        {
            InitController();
            string xmGuid = "testxmGuid";
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid"
            };
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);

            _personLogic.Setup(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>())).Returns(GetFakexmPerson());
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Throws(new Exception("", new NotFoundException()));
            _personLogic.Setup(x => x.UpdatePersonTimeZone(It.IsAny<Person>())).Returns(true);
            _personLogic.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Returns(GetFakePrimaryDeviceResponse());
            _personLogic.Setup(x => x.RemoveDevice(It.IsAny<DeviceRequest>())).Returns(GetFakePrimaryDeviceResponse());
            _personLogic.Setup(x => x.SavePersonNotificationGroups(It.IsAny<List<NotificationGroup>>(), It.IsAny<List<NotificationGroup>>()));
            JsonResult result = _controller.SaveNotification(GetFakeViewModel());
            Assert.AreEqual("success", result.Data);
            _personLogic.Verify(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>()), Times.Once);
            _personLogic.Verify(x => x.GetPersonByTargetName(It.IsAny<string>()), Times.Once);
            _personLogic.Verify(x => x.CreateDevice(It.IsAny<DeviceRequest>()), Times.Never);
        }

        [Test]
        public void SaveNotification_TimezoneChange()
        {
            var persons = new Persons() { Id = xmGuid, TimeZone = "US/Mountain" };
            InitController(persons);
            PersonDevices devices = null;
            GroupMembers xmGroups = null;
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid"
            };
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            var request = GetFakeViewModel();
            request.Person.TimeZone = "Asia/Hong_Kong";

            _personLogic.Setup(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>())).Returns(GetFakexmPerson());
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Throws(new NotFoundException());
            _personLogic.Setup(x => x.UpdatePersonTimeZone(It.IsAny<Person>())).Returns(true);
            _personLogic.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Returns(GetFakePrimaryDeviceResponse());
            _personLogic.Setup(x => x.RemoveDevice(It.IsAny<DeviceRequest>())).Returns(GetFakePrimaryDeviceResponse());
            _personLogic.Setup(x => x.SavePersonNotificationGroups(It.IsAny<List<NotificationGroup>>(), It.IsAny<List<NotificationGroup>>()));
            JsonResult result = _controller.SaveNotification(request);
            Assert.AreEqual("success", result.Data);
            _personLogic.Verify(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>()), Times.Never);
            _personLogic.Verify(x => x.UpdatePersonTimeZone(It.IsAny<Person>()), Times.Once);
            _personLogic.Verify(x => x.GetPersonByTargetName(It.IsAny<string>()), Times.Never);
            _personLogic.Verify(x => x.CreateDevice(It.IsAny<DeviceRequest>()), Times.Never);
        }

        [Test]
        public void SaveNotification_DeviceAddedAndRemoved()
        {
            var persons = new Persons() { Id = xmGuid, TimeZone = "US/Mountain" };
            var devices = new List<Device>(){
                new Device
                {
                    Id=primaryDeviceId,
                    Name = "Work Email",
                    DeviceType = "EMAIL",
                    Description = "test@test.com",
                    EmailAddress = "test@test.com"
                },
                new Device
                {
                    Id = smsDeviceId,
                    Name = "SMS Phone",
                    DeviceType = "TEXT_PHONE",
                    Description = "test2@test.com",
                    EmailAddress = "test2@test.com"
                }
            };
            InitController(persons, devices);
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid"
            };
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            var request = GetFakeViewModel();
            request.Devices.Add(GetFakeSecondaryDevice());
            request.Person.TimeZone = "Asia/Hong_Kong";

            _personLogic.Setup(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>())).Returns(GetFakexmPerson());
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Throws(new NotFoundException());
            _personLogic.Setup(x => x.UpdatePersonTimeZone(It.IsAny<Person>())).Returns(true);
            _personLogic.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Returns(GetFakeSecondaryDeviceResponse());
            _personLogic.Setup(x => x.RemoveDevice(It.IsAny<DeviceRequest>())).Returns(GetFakeSMSPhoneDeviceResponse());
            _personLogic.Setup(x => x.SavePersonNotificationGroups(It.IsAny<List<NotificationGroup>>(), It.IsAny<List<NotificationGroup>>()));
            JsonResult result = _controller.SaveNotification(request);
            Assert.AreEqual("success", result.Data);
            _personLogic.Verify(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>()), Times.Never);
            _personLogic.Verify(x => x.UpdatePersonTimeZone(It.IsAny<Person>()), Times.Once);
            _personLogic.Verify(x => x.GetPersonByTargetName(It.IsAny<string>()), Times.Never);
            _personLogic.Verify(x => x.CreateDevice(It.IsAny<DeviceRequest>()), Times.Once);
            _personLogic.Verify(x => x.RemoveDevice(It.IsAny<DeviceRequest>()), Times.Once);
        }

        [Test]
        public void SaveNotification_NewDevices()
        {
            var persons = new Persons() { Id = xmGuid, TimeZone = "US/Mountain" };
            List<Device> devices = new List<Device>();
            InitController(persons, devices);
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid"
            };
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            var request = GetFakeViewModel();
            request.Devices[0].Id = null;
            request.Person.TimeZone = "Asia/Hong_Kong";

            _personLogic.Setup(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>())).Returns(GetFakexmPerson());
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Throws(new NotFoundException());
            _personLogic.Setup(x => x.UpdatePersonTimeZone(It.IsAny<Person>())).Returns(true);
            _personLogic.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Returns(GetFakePrimaryDeviceResponse());
            _personLogic.Setup(x => x.RemoveDevice(It.IsAny<DeviceRequest>())).Returns(GetFakeSMSPhoneDeviceResponse());
            _personLogic.Setup(x => x.SavePersonNotificationGroups(It.IsAny<List<NotificationGroup>>(), It.IsAny<List<NotificationGroup>>()));
            JsonResult result = _controller.SaveNotification(request);
            Assert.AreEqual("success", result.Data);
            _personLogic.Verify(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>()), Times.Never);
            _personLogic.Verify(x => x.UpdatePersonTimeZone(It.IsAny<Person>()), Times.Once);
            _personLogic.Verify(x => x.GetPersonByTargetName(It.IsAny<string>()), Times.Never);
            _personLogic.Verify(x => x.CreateDevice(It.IsAny<DeviceRequest>()), Times.Once);
            _personLogic.Verify(x => x.RemoveDevice(It.IsAny<DeviceRequest>()), Times.Never);
        }

        [Test]
        public void SaveNotification_ChangeDevices()
        {
            var persons = new Persons() { Id = xmGuid, TimeZone = "US/Mountain" };
            List<Device> devices = new List<Device>();
            devices.Add(GetFakeSecondaryDevice());
            InitController(persons, devices);
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid"
            };
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            var request = GetFakeViewModel();
            request.Devices[0].Id = null;
            request.Person.TimeZone = "Asia/Hong_Kong";

            _personLogic.Setup(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>())).Returns(GetFakexmPerson());
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Throws(new NotFoundException());
            _personLogic.Setup(x => x.UpdatePersonTimeZone(It.IsAny<Person>())).Returns(true);
            _personLogic.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Returns(GetFakePrimaryDeviceResponse());
            _personLogic.Setup(x => x.RemoveDevice(It.IsAny<DeviceRequest>())).Returns(GetFakeSecondaryDeviceResponse());
            _personLogic.Setup(x => x.SavePersonNotificationGroups(It.IsAny<List<NotificationGroup>>(), It.IsAny<List<NotificationGroup>>()));
            JsonResult result = _controller.SaveNotification(request);
            Assert.AreEqual("success", result.Data);
            _personLogic.Verify(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>()), Times.Never);
            _personLogic.Verify(x => x.UpdatePersonTimeZone(It.IsAny<Person>()), Times.Once);
            _personLogic.Verify(x => x.GetPersonByTargetName(It.IsAny<string>()), Times.Never);
            _personLogic.Verify(x => x.CreateDevice(It.IsAny<DeviceRequest>()), Times.Once);
            _personLogic.Verify(x => x.RemoveDevice(It.IsAny<DeviceRequest>()), Times.Once);
        }

        [Test]
        public void SaveNotification_UpdateDevices()
        {
            var persons = new Persons() { Id = xmGuid, TimeZone = "US/Mountain" };
            List<Device> devices = GetFakeDevices();
            devices[0].EmailAddress = "testChange@test.com";
            InitController(persons, devices);
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid"
            };
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            var request = GetFakeViewModel();
            request.Person.TimeZone = "Asia/Hong_Kong";

            _personLogic.Setup(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>())).Returns(GetFakexmPerson());
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Throws(new NotFoundException());
            _personLogic.Setup(x => x.UpdatePersonTimeZone(It.IsAny<Person>())).Returns(true);
            _personLogic.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Returns(GetFakePrimaryDeviceResponse());
            _personLogic.Setup(x => x.RemoveDevice(It.IsAny<DeviceRequest>())).Returns(GetFakeSecondaryDeviceResponse());
            _personLogic.Setup(x => x.SavePersonNotificationGroups(It.IsAny<List<NotificationGroup>>(), It.IsAny<List<NotificationGroup>>()));
            JsonResult result = _controller.SaveNotification(request);
            Assert.AreEqual("success", result.Data);
            _personLogic.Verify(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>()), Times.Never);
            _personLogic.Verify(x => x.UpdatePersonTimeZone(It.IsAny<Person>()), Times.Once);
            _personLogic.Verify(x => x.GetPersonByTargetName(It.IsAny<string>()), Times.Never);
            _personLogic.Verify(x => x.CreateDevice(It.IsAny<DeviceRequest>()), Times.Once);
            _personLogic.Verify(x => x.RemoveDevice(It.IsAny<DeviceRequest>()), Times.Never);
        }

        [Test]
        public void SaveNotification_UpdateDevices_InvalidPrimaryEmail()
        {
            var persons = new Persons() { Id = xmGuid, TimeZone = "US/Mountain" };
            List<Device> devices = GetFakeDevices();

            InitController(persons, devices);
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid"
            };
            var request = GetFakeViewModel();
            request.Devices[0].Id = null;

            PersonDevices persondevices = GetPersonDevices();
            GroupMembers xmGroups = null;
            person.Id = xmGuid;
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            List<NotificationGroup> groups = GetNotificationGroups();
            _personLogic.Setup(x => x.GetPersonDevices(xmGuid)).Returns(persondevices);
            _personLogic.Setup(x => x.GetPerson(xmGuid)).Returns(person);
            _personLogic.Setup(x => x.UpdateNotificationProfile(adfsGuid, xmGuid));
            _personLogic.Setup(x => x.GetSFNotificationProfile(adfsGuid)).Returns(detail);
            _personLogic.Setup(x => x.GetPersonNotificationGroups(person, persondevices, xmGroups, detail.Events_Notifications, detail.Maintainance_Notifications, detail.NotificationContacts)).Returns(groups);
            _personLogic.Setup(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>())).Returns(GetFakexmPerson());
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Throws(new NotFoundException());
            _personLogic.Setup(x => x.UpdatePersonTimeZone(It.IsAny<Person>())).Returns(true);
            _personLogic.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Throws(new Exception("Invalid Primary Email Address", new FormatException()));
            _personLogic.Setup(x => x.RemoveDevice(It.IsAny<DeviceRequest>())).Returns(GetFakeSecondaryDeviceResponse());
            _personLogic.Setup(x => x.SavePersonNotificationGroups(It.IsAny<List<NotificationGroup>>(), It.IsAny<List<NotificationGroup>>()));
            JsonResult result = _controller.SaveNotification(request);
            Assert.AreEqual("invalidPrimaryEmail", result.Data);
        }

        [Test]
        public void SaveNotification_UpdateDevices_UnknownException()
        {
            var persons = new Persons() { Id = xmGuid, TimeZone = "US/Mountain" };
            List<Device> devices = GetFakeDevices();

            InitController(persons, devices);
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid"
            };
            var request = GetFakeViewModel();
            request.Devices[0].Id = null;

            PersonDevices persondevices = GetPersonDevices();
            GroupMembers xmGroups = null;
            person.Id = xmGuid;
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            List<NotificationGroup> groups = GetNotificationGroups();
            _personLogic.Setup(x => x.GetPersonDevices(xmGuid)).Returns(persondevices);
            _personLogic.Setup(x => x.GetPerson(xmGuid)).Returns(person);
            _personLogic.Setup(x => x.UpdateNotificationProfile(adfsGuid, xmGuid));
            _personLogic.Setup(x => x.GetSFNotificationProfile(adfsGuid)).Returns(detail);
            _personLogic.Setup(x => x.GetPersonNotificationGroups(person, persondevices, xmGroups, detail.Events_Notifications, detail.Maintainance_Notifications, detail.NotificationContacts)).Returns(groups);
            _personLogic.Setup(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>())).Returns(GetFakexmPerson());
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Throws(new NotFoundException());
            _personLogic.Setup(x => x.UpdatePersonTimeZone(It.IsAny<Person>())).Returns(true);
            _personLogic.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Throws(new Exception("Invalid Information", new FormatException()));
            _personLogic.Setup(x => x.RemoveDevice(It.IsAny<DeviceRequest>())).Returns(GetFakeSecondaryDeviceResponse());
            _personLogic.Setup(x => x.SavePersonNotificationGroups(It.IsAny<List<NotificationGroup>>(), It.IsAny<List<NotificationGroup>>()));
            Assert.Throws<Exception>(() => _controller.SaveNotification(request));
        }

        [Test]
        public void SaveNotification_UpdateDevices_InvalidSecondaryEmail()
        {
            var persons = new Persons() { Id = xmGuid, TimeZone = "US/Mountain" };
            List<Device> devices = new List<Device>();
            InitController(persons, devices);
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid"
            };
            var request = GetFakeViewModel();
            request.Devices = new List<Device>();
            request.Devices.Add(GetFakeSecondaryDevice());
            PersonDevices persondevices = GetPersonDevices();
            GroupMembers xmGroups = null;
            person.Id = xmGuid;
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            List<NotificationGroup> groups = GetNotificationGroups();
            _personLogic.Setup(x => x.GetPersonDevices(xmGuid)).Returns(persondevices);
            _personLogic.Setup(x => x.GetPerson(xmGuid)).Returns(person);
            _personLogic.Setup(x => x.UpdateNotificationProfile(adfsGuid, xmGuid));
            _personLogic.Setup(x => x.GetSFNotificationProfile(adfsGuid)).Returns(detail);
            _personLogic.Setup(x => x.GetPersonNotificationGroups(person, persondevices, xmGroups, detail.Events_Notifications, detail.Maintainance_Notifications, detail.NotificationContacts)).Returns(groups);
            _personLogic.Setup(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>())).Returns(GetFakexmPerson());
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Throws(new NotFoundException());
            _personLogic.Setup(x => x.UpdatePersonTimeZone(It.IsAny<Person>())).Returns(true);
            _personLogic.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Throws(new Exception("Invalid Secondary Email Address" ,new FormatException()));
            _personLogic.Setup(x => x.RemoveDevice(It.IsAny<DeviceRequest>())).Returns(GetFakeSecondaryDeviceResponse());
            _personLogic.Setup(x => x.SavePersonNotificationGroups(It.IsAny<List<NotificationGroup>>(), It.IsAny<List<NotificationGroup>>()));
            JsonResult result = _controller.SaveNotification(request);
            Assert.AreEqual("invalidSecondaryEmail", result.Data);
        }

        [Test]
        public void SaveNotification_UpdateDevices_InvalidSMSPhone()
        {
            var persons = new Persons() { Id = xmGuid, TimeZone = "US/Mountain" };
            List<Device> devices = new List<Device>();
            InitController(persons, devices);
            GetPersonResponse person = new GetPersonResponse()
            {
                Id = "testxmGuid"
            };
            var request = GetFakeViewModel();
            request.Devices = new List<Device>();
            request.Devices.Add(GetFakeSMSPhoneDevice());
            PersonDevices persondevices = GetPersonDevices();
            GroupMembers xmGroups = null;
            person.Id = xmGuid;
            UserNotificationDetail detail = GetUserNotificationDetail(xmGuid);
            List<NotificationGroup> groups = GetNotificationGroups();
            _personLogic.Setup(x => x.GetPersonDevices(xmGuid)).Returns(persondevices);
            _personLogic.Setup(x => x.GetPerson(xmGuid)).Returns(person);
            _personLogic.Setup(x => x.UpdateNotificationProfile(adfsGuid, xmGuid));
            _personLogic.Setup(x => x.GetSFNotificationProfile(adfsGuid)).Returns(detail);
            _personLogic.Setup(x => x.GetPersonNotificationGroups(person, persondevices, xmGroups, detail.Events_Notifications, detail.Maintainance_Notifications, detail.NotificationContacts)).Returns(groups);
            _personLogic.Setup(x => x.CreateXmattersPerson(It.IsAny<string>(), It.IsAny<Person>())).Returns(GetFakexmPerson());
            _personLogic.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Throws(new NotFoundException());
            _personLogic.Setup(x => x.UpdatePersonTimeZone(It.IsAny<Person>())).Returns(true);
            _personLogic.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Throws(new Exception("Invalid SMS phone number" , new FormatException()));
            _personLogic.Setup(x => x.RemoveDevice(It.IsAny<DeviceRequest>())).Returns(GetFakeSecondaryDeviceResponse());
            _personLogic.Setup(x => x.SavePersonNotificationGroups(It.IsAny<List<NotificationGroup>>(), It.IsAny<List<NotificationGroup>>()));
            JsonResult result = _controller.SaveNotification(request);
            Assert.AreEqual("invalidPhoneNumber", result.Data);
        }

        #region Private Methods

        private void InitController(Persons person = null, List<Device> devices = null)
        {
            List<Claim> claims = new List<Claim>();
            Claim adfsGuidClaim = new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, Convert.ToBase64String(new Guid(adfsGuid).ToByteArray()));
            claims.Add(adfsGuidClaim);
            Claim userNameClaim = new Claim(System.Security.Claims.ClaimTypes.Name, "TestUserName");
            claims.Add(userNameClaim);

            NotificationManagerViewModel sessionModel = new NotificationManagerViewModel();
            if (person == null)
                sessionModel.Person = new Persons()
                {
                    TimeZone = "US/Mountain"
                };
            else
                sessionModel.Person = person;
            sessionModel.NotificationGroups = GetFakeNotificationGroups1();
            if (devices == null)
                sessionModel.Devices = GetFakeDevices();
            else
                sessionModel.Devices = devices.Count == 0 ? null : devices;

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

        private NotificationManagerViewModel GetFakeViewModel()
        {
            NotificationManagerViewModel viewModel = new NotificationManagerViewModel
            {
                Person = new Persons()
                {
                    Id = xmGuid,
                    TargetName = "tester",
                    FirstName = "Test",
                    LastName = "User",
                    Status = "ACTIVE",
                    TimeZone = "US/Pacific",
                },
                NotificationGroups = GetFakeNotificationGroups1(),
                Devices = new List<Device>()
            {
                new Device()
                {
                    Id = primaryDeviceId,
                    DeviceType = "EMAIL",
                    Description = "test@test.com",
                    Name="Work Email",
                    EmailAddress = "test@test.com"
                }
            }
            };

            return viewModel;
        }

        private Persons GetFakePersons()
        {
            return new Persons()
            {
                Id = xmGuid,
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                Status = "ACTIVE",
                TimeZone = "US/Pacific",
            };
        }

        private List<Device> GetFakeDevices()
        {
            List<Device> device = new List<Device>()
            {
                new Device()
                {
                    Id = primaryDeviceId,
                    DeviceType = "EMAIL",
                    Description = "test@test.com",
                    Name="Work Email",
                    EmailAddress = "test@test.com"
                }
            };
            return device;
        }

        private DeviceResponse GetFakePrimaryDeviceResponse()
        {
            DeviceResponse device = new DeviceResponse()
            {
                Id = primaryDeviceId,
                DeviceType = "EMAIL",
                Description = "test@test.com",
                Name = "Work Email",
                EmailAddress = "test@test.com"
            };
            return device;
        }

        private Device GetFakeSecondaryDevice()
        {
            Device device = new Device()
            {
                DeviceType = "EMAIL",
                Description = "test@test.com",
                Name = "Secondary Email",
                EmailAddress = "test@test.com"
            };
            return device;
        }

        private DeviceRequest GetFakeSecondaryDeviceRequest()
        {
            DeviceRequest device = new DeviceRequest()
            {
                DeviceType = "EMAIL",
                Description = "test@test.com",
                Name = "Secondary Email",
                EmailAddress = "test@test.com"
            };
            return device;
        }

        private DeviceResponse GetFakeSecondaryDeviceResponse()
        {
            DeviceResponse device = new DeviceResponse()
            {
                Id = primaryDeviceId,
                DeviceType = "EMAIL",
                Description = "test@test.com",
                Name = "Secondary Email",
                EmailAddress = "test@test.com"
            };
            return device;
        }

        private Device GetFakeSMSPhoneDevice()
        {
            Device device = new Device()
            {
                DeviceType = "TEXT_PHONE",
                Description = "9123456789",
                Name = "SMS Phone",
                PhoneNumber = "+19123456789",
                CountryCode = "US",
                DeviceTimeFrame = new List<TimeFrame>()
                {
                    new TimeFrame()
                    {
                        Name = "24x7",
                        StartTime = "00:00",
                        EndTime = "00:00",
                        DurationInMinutes = 1440
                    }
                }
            };
            return device;
        }

        private DeviceResponse GetFakeSMSPhoneDeviceResponse()
        {
            DeviceResponse device = new DeviceResponse()
            {
                Id = smsDeviceId,
                DeviceType = "TEXT_PHONE",
                Description = "9123456789",
                Name = "SMS Phone",
                PhoneNumber = "+19123456789",
                Country = "US",
                TimeFrames = GetTimeFrame()
            };
            return device;
        }

        private List<NotificationGroup> GetFakeNotificationGroups1()
        {
            List<NotificationGroup> notificationGroups = new List<NotificationGroup>();
            List<DeviceNotifications> deviceNotifications = new List<DeviceNotifications>()
            {
                new DeviceNotifications()
                {
                    DeviceId = primaryDeviceId,
                    DeviceType = "Work Email",
                    IsEventsSubscribed = true,
                    IsMaintainanceSubscribed = true
                },
            };
            notificationGroups.Add(new NotificationGroup() { AccountBU = "BU1", DeviceNotifications = deviceNotifications, EventsGroupName = "EventGN1", MaintainanceGroupName = "MaintGN1", ProductCluster = "PC1" });
            notificationGroups.Add(new NotificationGroup() { AccountBU = "BU2", DeviceNotifications = deviceNotifications, EventsGroupName = "EventGN2", MaintainanceGroupName = "MaintGN2", ProductCluster = "PC2" });
            notificationGroups.Add(new NotificationGroup() { AccountBU = "BU3", DeviceNotifications = deviceNotifications, EventsGroupName = "EventGN3", MaintainanceGroupName = "MaintGN3", ProductCluster = "PC3" });

            return notificationGroups;
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

        private GetPersonResponse GetFakePerson()
        {
            return new GetPersonResponse
            {
                Id = xmGuid,
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                Status = "ACTIVE",
                RecipientType = "PERSON",
                Timezone = "US/Pacific"
            };
        }

        private Person GetFakexmPerson()
        {
            return new Person
            {
                Id = new Guid(xmGuid),
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                Status = "ACTIVE",
                RecipientType = "PERSON",
                Timezone = "US/Pacific"
            };
        }


        private PersonDevices GetPersonDevices()
        {
            PersonDevices devices = new PersonDevices
            {
                Devices = new List<DeviceResponse>()
            };
            devices.Devices.Add(new DeviceResponse()
            {
                DeviceType = "EMAIL",
                Id = primaryDeviceId,
                Name = "Work Email",
                EmailAddress = "test@test.com",
                Description = "test@test.com"
            });
            return devices;
        }

        private List<NotificationGroup> GetNotificationGroups()
        {
            List<NotificationGroup> groups = new List<NotificationGroup>();
            groups.Add(new NotificationGroup() { EventsGroupName = "TestGroup" });
            return groups;
        }

        private static Timeframe GetTimeFrame()
        {
            string[] days = { "MO", "TU", "WE", "TH", "FR", "SA", "SU" };
            var tFrame = new Timeframe
            {
                data = new List<TimeFramedetails>()
                {
                    new TimeFramedetails()
                    {
                        name="24x7",
                        startTime = "00:00",
                        durationInMinutes = 1440,
                        days = new List<string>(days)
                    }
                }
            };
            return tFrame;
        }
        #endregion
    }
}
