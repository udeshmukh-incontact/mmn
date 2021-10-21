using ManageMyNotificationsBusinessLayer;
using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using ManageMyNotificationsBusinessLayer.Interfaces.XMatters;
using ManageMyNotificationsBusinessLayer.Proxy;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace ManageMyNotificationsBusinessLayerTests
{
    [TestFixture]
    public class PersonBusinessLogicTests
    {
        private readonly Guid _groupId = Guid.NewGuid();
        [Test]
        public void PersonBusinessLogic_GetPersonNotificationGroups_Calls_InternalMethods()
        {
            GetPersonResponse person = getPersonObject();
            PersonDevices personDevices = getPersonDevices();
            var eventNotifications = getEventNotifications();
            var maintenanceNotifications = getMaintenanceNotifications();
            var members = getGroupMembers();

            var _xMatterPersonService = new Mock<IXMatterPersonService>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _xMatterPersonService.Setup(x => x.GetPersonGroups(It.IsAny<string>(), false)).Returns(members);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(eventNotifications);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(maintenanceNotifications);

            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            string adfsGuid = "23384";

            var notificationContacts = new List<NotificationContact>();
            var newNotificationContact = new NotificationContact
            {
                Id = "Id",
                FirstName = "FirstName",
                LastName = "LastName",
                AccountId = "AccountId",
                XMPersonId = "XMPersonId",
                XMPersonGuid = "XMPersonGuid",
                AdfsGuid = adfsGuid,
                Deactivated = false,
                AccountRootParentIDs = new string[] {"1234","21341234"}
            };
            notificationContacts.Add(newNotificationContact);

            _sfNotificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Returns(notificationContacts.ToArray());

            PersonBusinessLogic personLogic = new PersonBusinessLogic(_xMatterPersonService.Object, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            List<NotificationGroup> notificationGroups = personLogic.GetPersonNotificationGroups(person, personDevices, members, eventNotifications, maintenanceNotifications, notificationContacts);
            _xMatterPersonService.Verify(x => x.GetPersonGroups(It.IsAny<string>(), false), Times.Never);
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType"), Times.Never);
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType"), Times.Never);
        }
        [Test]
        public void PersonBusinessLogic_GetPersonNotificationGroups_Calls_GroupMembers_Null()
        {
            GetPersonResponse person = getPersonObject();
            PersonDevices personDevices = null;
            var eventNotifications = getEventNotifications();
            var maintenanceNotifications = getMaintenanceNotifications();
            GroupMembers members = null;

            var _xMatterPersonService = new Mock<IXMatterPersonService>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _xMatterPersonService.Setup(x => x.GetPersonGroups(It.IsAny<string>(), false)).Returns(members);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(eventNotifications);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(maintenanceNotifications);

            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            string adfsGuid = "23384";

            var notificationContacts = new List<NotificationContact>();
            var newNotificationContact = new NotificationContact
            {
                Id = "Id",
                FirstName = "FirstName",
                LastName = "LastName",
                AccountId = "AccountId",
                XMPersonId = "XMPersonId",
                XMPersonGuid = "XMPersonGuid",
                AdfsGuid = adfsGuid,
                Deactivated = false
            };
            notificationContacts.Add(newNotificationContact);

            _sfNotificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Returns(notificationContacts.ToArray());

            PersonBusinessLogic personLogic = new PersonBusinessLogic(_xMatterPersonService.Object, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            List<NotificationGroup> notificationGroups = personLogic.GetPersonNotificationGroups(person, personDevices, members, eventNotifications, maintenanceNotifications, notificationContacts);
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType"), Times.Never);
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType"), Times.Never);
        }

        [Test]
        public void PersonBusinessLogic_GetPersonNotificationGroups_Returns_CorrectNotificationGroupsCount()
        {
            GetPersonResponse person = getPersonObject();
            PersonDevices personDevices = getPersonDevices();
            var eventNotifications = getEventNotifications();
            var maintenanceNotifications = getMaintenanceNotifications();
            //var members = getGroupMembers();
            GroupMember member1 = new GroupMember() { Group = new Group() { TargetName = "Group1" }, Member = new Member() { Id = "Id1" } };
            GroupMember member2 = new GroupMember() { Group = new Group() { TargetName = "Group2" }, Member = new Member() { Id = "Id2" } };
            var member = new List<GroupMember>();
            member.Add(member1);
            member.Add(member2);
            var members = new GroupMembers() { GroupMember = member };

            var _xMatterPersonService = new Mock<IXMatterPersonService>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _xMatterPersonService.Setup(x => x.GetPersonGroups(It.IsAny<string>(), false)).Returns(members);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(eventNotifications);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(maintenanceNotifications);

            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            string adfsGuid = "23384";

            var notificationContacts = new List<NotificationContact>();
            var newNotificationContact = new NotificationContact
            {
                Id = "Id",
                FirstName = "FirstName",
                LastName = "LastName",
                AccountId = "AccountId",
                XMPersonId = "XMPersonId",
                XMPersonGuid = "XMPersonGuid",
                AdfsGuid = adfsGuid,
                Deactivated = false
            };
            notificationContacts.Add(newNotificationContact);

            _sfNotificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Returns(notificationContacts.ToArray());

            PersonBusinessLogic personLogic = new PersonBusinessLogic(_xMatterPersonService.Object, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            List<NotificationGroup> notificationGroups = personLogic.GetPersonNotificationGroups(person, personDevices, members, eventNotifications, maintenanceNotifications, notificationContacts);
            Assert.AreEqual(notificationGroups.Count, 3);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group1").DeviceNotifications.Count, 3);
        }

        [Test]
        public void PersonBusinessLogic_GetPersonNotificationGroups_SecondaryEmailDevice_EventsAndMaintenanceValueTrue()
        {
            GetPersonResponse person = getPersonObject();
            PersonDevices personDevices = getPersonDevices();
            var eventNotifications = getEventNotifications();
            var maintenanceNotifications = getMaintenanceNotifications();
            //var members = getGroupMembers();
            GroupMember member1 = new GroupMember() { Group = new Group() { TargetName = "Group1" }, Member = new Member() { Id = "Id1" } };
            GroupMember member2 = new GroupMember() { Group = new Group() { TargetName = "Group2" }, Member = new Member() { Id = "Id2" } };
            var member = new List<GroupMember>();
            member.Add(member1);
            member.Add(member2);
            var members = new GroupMembers() { GroupMember = member };

            var _xMatterPersonService = new Mock<IXMatterPersonService>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _xMatterPersonService.Setup(x => x.GetPersonGroups(It.IsAny<string>(), false)).Returns(members);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(eventNotifications);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(maintenanceNotifications);

            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            string adfsGuid = "23384";

            var notificationContacts = new List<NotificationContact>();
            var newNotificationContact = new NotificationContact
            {
                Id = "Id",
                FirstName = "FirstName",
                LastName = "LastName",
                AccountId = "AccountId",
                XMPersonId = "XMPersonId",
                XMPersonGuid = "XMPersonGuid",
                AdfsGuid = adfsGuid,
                Deactivated = false
            };
            notificationContacts.Add(newNotificationContact);

            _sfNotificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Returns(notificationContacts.ToArray());

            PersonBusinessLogic personLogic = new PersonBusinessLogic(_xMatterPersonService.Object, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            List<NotificationGroup> notificationGroups = personLogic.GetPersonNotificationGroups(person, personDevices, members, eventNotifications, maintenanceNotifications, notificationContacts);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group1").DeviceNotifications.Find(x => x.DeviceType == "Secondary Email").IsEventsSubscribed, true);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group1").DeviceNotifications.Find(x => x.DeviceType == "Secondary Email").IsMaintainanceSubscribed, true);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group1").DeviceNotifications.Find(x => x.DeviceType == "Work Email").IsEventsSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group1").DeviceNotifications.Find(x => x.DeviceType == "Work Email").IsMaintainanceSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group1").DeviceNotifications.Find(x => x.DeviceType == "SMS Phone").IsEventsSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group1").DeviceNotifications.Find(x => x.DeviceType == "SMS Phone").IsMaintainanceSubscribed, false);
        }

        [Test]
        public void PersonBusinessLogic_GetPersonNotificationGroups_WorkEmailDevice_EventsAndMaintenanceValueTrue()
        {
            GetPersonResponse person = getPersonObject();
            PersonDevices personDevices = getPersonDevices();
            var eventNotifications = getEventNotifications();
            var maintenanceNotifications = getMaintenanceNotifications();
            //var members = getGroupMembers();
            GroupMember member1 = new GroupMember() { Group = new Group() { TargetName = "Group1" }, Member = new Member() { Id = "Id1" } };
            GroupMember member2 = new GroupMember() { Group = new Group() { TargetName = "Group2" }, Member = new Member() { Id = "Id2" } };
            var member = new List<GroupMember>();
            member.Add(member1);
            member.Add(member2);
            var members = new GroupMembers() { GroupMember = member };

            var _xMatterPersonService = new Mock<IXMatterPersonService>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _xMatterPersonService.Setup(x => x.GetPersonGroups(It.IsAny<string>(), false)).Returns(members);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(eventNotifications);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(maintenanceNotifications);

            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            string adfsGuid = "23384";

            var notificationContacts = new List<NotificationContact>();
            var newNotificationContact = new NotificationContact
            {
                Id = "Id",
                FirstName = "FirstName",
                LastName = "LastName",
                AccountId = "AccountId",
                XMPersonId = "XMPersonId",
                XMPersonGuid = "XMPersonGuid",
                AdfsGuid = adfsGuid,
                Deactivated = false
            };
            notificationContacts.Add(newNotificationContact);

            _sfNotificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Returns(notificationContacts.ToArray());

            PersonBusinessLogic personLogic = new PersonBusinessLogic(_xMatterPersonService.Object, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            List<NotificationGroup> notificationGroups = personLogic.GetPersonNotificationGroups(person, personDevices, members, eventNotifications, maintenanceNotifications, notificationContacts);

            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group2").DeviceNotifications.Find(x => x.DeviceType == "Secondary Email").IsEventsSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group2").DeviceNotifications.Find(x => x.DeviceType == "Secondary Email").IsMaintainanceSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group2").DeviceNotifications.Find(x => x.DeviceType == "Work Email").IsEventsSubscribed, true);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group2").DeviceNotifications.Find(x => x.DeviceType == "Work Email").IsMaintainanceSubscribed, true);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group2").DeviceNotifications.Find(x => x.DeviceType == "SMS Phone").IsEventsSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group2").DeviceNotifications.Find(x => x.DeviceType == "SMS Phone").IsMaintainanceSubscribed, false);
        }

        [Test]
        public void PersonBusinessLogic_GetPersonNotificationGroups_AllDevices_EventsAndMaintenanceValueFalse()
        {
            GetPersonResponse person = getPersonObject();
            PersonDevices personDevices = getPersonDevices();
            var eventNotifications = getEventNotifications();
            var maintenanceNotifications = getMaintenanceNotifications();
            //var members = getGroupMembers();
            GroupMember member1 = new GroupMember() { Group = new Group() { TargetName = "Group1" }, Member = new Member() { Id = "Id1" } };
            GroupMember member2 = new GroupMember() { Group = new Group() { TargetName = "Group2" }, Member = new Member() { Id = "Id2" } };
            var member = new List<GroupMember>();
            member.Add(member1);
            member.Add(member2);
            var members = new GroupMembers() { GroupMember = member };

            var _xMatterPersonService = new Mock<IXMatterPersonService>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _xMatterPersonService.Setup(x => x.GetPersonGroups(It.IsAny<string>(), false)).Returns(members);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(eventNotifications);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(maintenanceNotifications);

            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            string adfsGuid = "23384";

            var notificationContacts = new List<NotificationContact>();
            var newNotificationContact1 = new NotificationContact
            {
                Id = "Id",
                FirstName = "FirstName",
                LastName = "LastName",
                AccountId = "1",
                AccountRootParentIDs = new string[] { "x1" },
                XMPersonId = "XMPersonId",
                XMPersonGuid = "XMPersonGuid",
                AdfsGuid = adfsGuid,
                Deactivated = false
            };
            var newNotificationContact2 = new NotificationContact
            {
                Id = "Id",
                FirstName = "FirstName2",
                LastName = "LastName2",
                AccountId = "test",
                XMPersonId = "XMPersonId",
                XMPersonGuid = "XMPersonGuid",
                AdfsGuid = adfsGuid,
                Deactivated = false
            };
            notificationContacts.Add(newNotificationContact1);
            notificationContacts.Add(newNotificationContact2);

            _sfNotificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Returns(notificationContacts.ToArray());

            PersonBusinessLogic personLogic = new PersonBusinessLogic(_xMatterPersonService.Object, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            List<NotificationGroup> notificationGroups = personLogic.GetPersonNotificationGroups(person, personDevices, members, eventNotifications, maintenanceNotifications, notificationContacts);

            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group3").DeviceNotifications.Find(x => x.DeviceType == "Secondary Email").IsEventsSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group3").DeviceNotifications.Find(x => x.DeviceType == "Secondary Email").IsMaintainanceSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group3").DeviceNotifications.Find(x => x.DeviceType == "Work Email").IsEventsSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group3").DeviceNotifications.Find(x => x.DeviceType == "Work Email").IsMaintainanceSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group3").DeviceNotifications.Find(x => x.DeviceType == "SMS Phone").IsEventsSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group3").DeviceNotifications.Find(x => x.DeviceType == "SMS Phone").IsMaintainanceSubscribed, false);

        }

        [Test]
        public void PersonBusinessLogic_GetPersonNotificationGroups_AllDevices_EventsAndMaintenance_Throws_Exception()
        {
            GetPersonResponse person = getPersonObject();
            PersonDevices personDevices = getPersonDevices();
            var eventNotifications = getEventNotifications();
            var maintenanceNotifications = getMaintenanceNotifications();
            //var members = getGroupMembers();
            GroupMember member1 = new GroupMember() { Group = new Group() { TargetName = "Group1" }, Member = new Member() { Id = "Id1" } };
            GroupMember member2 = new GroupMember() { Group = new Group() { TargetName = "Group2" }, Member = new Member() { Id = "Id2" } };
            var member = new List<GroupMember>();
            member.Add(member1);
            member.Add(member2);
            var members = new GroupMembers() { GroupMember = member };
            List<NotificationContact> notificationContacts = null;
            var _xMatterPersonService = new Mock<IXMatterPersonService>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _xMatterPersonService.Setup(x => x.GetPersonGroups(It.IsAny<string>(), false)).Throws(new Exception());
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(eventNotifications);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(maintenanceNotifications);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(_xMatterPersonService.Object, null, _xaMatterIntegrationService.Object, null);
            Assert.Throws<Exception>(() => personLogic.GetPersonNotificationGroups(person, personDevices, members, eventNotifications, maintenanceNotifications, notificationContacts));
        }

        [Test]
        public void PersonBusinessLogic_GetPersonNotificationGroups_Contact_Deactivated()
        {
            GetPersonResponse person = getPersonObject();
            PersonDevices personDevices = getPersonDevices();
            var eventNotifications = getEventNotifications();
            var maintenanceNotifications = getMaintenanceNotifications();
            //var members = getGroupMembers();
            GroupMember member1 = new GroupMember() { Group = new Group() { TargetName = "Group1" }, Member = new Member() { Id = "Id1" } };
            GroupMember member2 = new GroupMember() { Group = new Group() { TargetName = "Group2" }, Member = new Member() { Id = "Id2" } };
            var member = new List<GroupMember>();
            member.Add(member1);
            member.Add(member2);
            var members = new GroupMembers() { GroupMember = member };

            var _xMatterPersonService = new Mock<IXMatterPersonService>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _xMatterPersonService.Setup(x => x.GetPersonGroups(It.IsAny<string>(), false)).Returns(members);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(eventNotifications);
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(maintenanceNotifications);

            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            string adfsGuid = "23384";

            var notificationContacts = new List<NotificationContact>();
            var newNotificationContact = new NotificationContact
            {
                Id = "Id",
                FirstName = "FirstName",
                LastName = "LastName",
                AccountId = "AccountId",
                XMPersonId = "XMPersonId",
                XMPersonGuid = "XMPersonGuid",
                AdfsGuid = adfsGuid,
                Deactivated = false
            };

            var newNotificationContactDeActivated = new NotificationContact
            {
                Id = "Id",
                FirstName = "FirstName",
                LastName = "LastName",
                AccountId = "AccountId",
                XMPersonId = "XMPersonId",
                XMPersonGuid = "XMPersonGuid",
                AdfsGuid = adfsGuid,
                Deactivated = true
            };
            notificationContacts.Add(newNotificationContact);
            notificationContacts.Add(newNotificationContactDeActivated);

            _sfNotificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Returns(notificationContacts.ToArray());

            PersonBusinessLogic personLogic = new PersonBusinessLogic(_xMatterPersonService.Object, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            List<NotificationGroup> notificationGroups = personLogic.GetPersonNotificationGroups(person, personDevices, members, eventNotifications, maintenanceNotifications, notificationContacts);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group2").DeviceNotifications.Find(x => x.DeviceType == "Secondary Email").IsEventsSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group2").DeviceNotifications.Find(x => x.DeviceType == "Secondary Email").IsMaintainanceSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group2").DeviceNotifications.Find(x => x.DeviceType == "Work Email").IsEventsSubscribed, true);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group2").DeviceNotifications.Find(x => x.DeviceType == "Work Email").IsMaintainanceSubscribed, true);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group2").DeviceNotifications.Find(x => x.DeviceType == "SMS Phone").IsEventsSubscribed, false);
            Assert.AreEqual(notificationGroups.Find(x => x.EventsGroupName == "Group2").DeviceNotifications.Find(x => x.DeviceType == "SMS Phone").IsMaintainanceSubscribed, false);
        }

        [Test]
        public void CreateDevices_Calls_XmatterPersonServiceMethod_AndReturns_DeviceResponse()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            var device = Device();
            xMatterPersonService.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Returns(device);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            var response = personLogic.CreateDevice(DeviceRequest());
            xMatterPersonService.Verify(x => x.CreateDevice(It.IsAny<DeviceRequest>()), Times.Once);
            Assert.That(Utilities.AreObjectsEquivalent(response, device));
        }

        [Test]
        public void CreateDevices_Calls_Set_Event_XmatterPersonServiceMethod_AndReturns_DeviceResponse()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            var device = Device();
            xMatterPersonService.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Returns(device);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            personLogic.OnDeviceSaved += OnDeviceDataSaved;
            var response = personLogic.CreateDevice(DeviceRequest());
            xMatterPersonService.Verify(x => x.CreateDevice(It.IsAny<DeviceRequest>()), Times.Once);
            Assert.That(Utilities.AreObjectsEquivalent(response, device));
        }

        [Test]
        public void CreateDevices_Calls__Device_Id_Null_XmatterPersonServiceMethod_AndReturns_DeviceResponse()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            var device = Device();
            var deviceRequest = DeviceRequest();
            deviceRequest.Id = null;
            xMatterPersonService.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Returns(device);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            var response = personLogic.CreateDevice(deviceRequest);
            xMatterPersonService.Verify(x => x.CreateDevice(It.IsAny<DeviceRequest>()), Times.Once);
            Assert.That(Utilities.AreObjectsEquivalent(response, device));
        }

        [Test]
        public void CreateDevices_Calls__Device_Id_Null_set_event_XmatterPersonServiceMethod_AndReturns_DeviceResponse()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            var device = Device();
            var deviceRequest = DeviceRequest();
            deviceRequest.Id = null;
            xMatterPersonService.Setup(x => x.CreateDevice(It.IsAny<DeviceRequest>())).Returns(device);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            personLogic.OnDeviceSaved += OnDeviceDataSaved;
            var response = personLogic.CreateDevice(deviceRequest);
            xMatterPersonService.Verify(x => x.CreateDevice(It.IsAny<DeviceRequest>()), Times.Once);
            Assert.That(Utilities.AreObjectsEquivalent(response, device));
        }

        [Test]
        public void CreateSalesforceProfile_Calls_NotificationService_CreateXmPersonMethod()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.CreateXMPersonAndUpdateContacts(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            personLogic.CreateSalesforceprofile("", "");
            notificationService.Verify(x => x.CreateXMPersonAndUpdateContacts(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void CreateSalesforceProfile_Calls_NotificationService_CreateXmPersonMethod_AndReturns_True()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.CreateXMPersonAndUpdateContacts(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            var returnValue = personLogic.CreateSalesforceprofile("", "");
            Assert.AreEqual(returnValue, true);
        }

        [Test]
        public void CreateSalesforceProfile_Calls_Throws_Exception()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.CreateXMPersonAndUpdateContacts(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            Assert.Throws<Exception>(() => personLogic.CreateSalesforceprofile("testId", "testId1"));
        }

        [Test]
        public void CreateSalesforceProfile_Calls_Throws_ServiceException()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.CreateXMPersonAndUpdateContacts(It.IsAny<string>(), It.IsAny<string>())).Throws(new EndpointNotFoundException());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            //Assert.Throws<ServiceException>(() => personLogic.CreateSalesforceprofile("testId", "testId1"));
            Assert.That(() => personLogic.CreateSalesforceprofile("testId", "testId1"), Throws.InnerException.TypeOf<EndpointNotFoundException>());
        }

        [Test]
        public void CreateXMattersPerson_Calls_PersonService_CreatePersonMethod()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.CreateXMPerson(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            var person = GetFakePersonObject();
            xMatterPersonService.Setup(x => x.CreatePerson(It.IsAny<Person>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, notificationService.Object, null, null);
            personLogic.CreateXmattersPerson("", person);
            xMatterPersonService.Verify(x => x.CreatePerson(person), Times.Once);
        }

        [Test]
        public void CreateXMattersPerson_Calls_PersonService_Set_Event_CreatePersonMethod()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.CreateXMPerson(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            var person = GetFakePersonObject();
            xMatterPersonService.Setup(x => x.CreatePerson(It.IsAny<Person>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, notificationService.Object, null, null);
            personLogic.OnPersonDataSaved += OnPersonDataSaved;
            personLogic.CreateXmattersPerson("", person);
            xMatterPersonService.Verify(x => x.CreatePerson(person), Times.Once);
        }

        [Test]
        public void CreateXMattersPerson_Calls_PersonService_CreatePersonMethod_AndReturns_Person()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.CreateXMPerson(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            var person = GetFakePersonObject();
            xMatterPersonService.Setup(x => x.CreatePerson(It.IsAny<Person>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, notificationService.Object, null, null);
            var returnPerson = personLogic.CreateXmattersPerson("", person);
            Assert.That(Utilities.AreObjectsEquivalent(returnPerson, person));
        }

        [Test]
        public void DisableXMatterProfile_Calls_NotificationService_GetXMPersonGuidMethod()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.GetXMPersonGuid(It.IsAny<string>())).Returns("");
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            personLogic.DisableXMattersProfile("", "");
            notificationService.Verify(x => x.GetXMPersonGuid(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void DisableXMatterProfile_Set_Event_Calls_NotificationService_GetXMPersonGuidMethod()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.GetXMPersonGuid(It.IsAny<string>())).Returns("");
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            personLogic.OnPersonDataSaved += OnPersonDataSaved;
            personLogic.DisableXMattersProfile("", "");
            notificationService.Verify(x => x.GetXMPersonGuid(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void DisableXMatterProfile_Calls_PersonService_UpdatePersonMethod()
        {
            var xmGuid = Guid.NewGuid();
            var adfsGuid = Guid.NewGuid();
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.GetXMPersonGuid(It.IsAny<string>())).Returns(Guid.NewGuid().ToString());
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            Person person = null;
            xMatterPersonService.Setup(x => x.UpdatePerson(It.IsAny<Person>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, notificationService.Object, null, null);
            personLogic.DisableXMattersProfile(adfsGuid.ToString(), xmGuid.ToString());
            xMatterPersonService.Verify(x => x.UpdatePerson(It.IsAny<Person>()), Times.Once);
        }

        [Test]
        public void DisableXMatterProfile_Calls_PersonService_ResultNotNullMethod()
        {
            var xmGuid = Guid.NewGuid();
            var adfsGuid = Guid.NewGuid();
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.GetXMPersonGuid(It.IsAny<string>())).Returns(Guid.NewGuid().ToString());
            //notificationService.Setup(x => x.DissociateNotificationContacts(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            Person person = new Person()
            {
                Id = Guid.NewGuid()
            };
            xMatterPersonService.Setup(x => x.UpdatePerson(It.IsAny<Person>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, notificationService.Object, null, null);
            personLogic.DisableXMattersProfile(adfsGuid.ToString(), xmGuid.ToString());
            xMatterPersonService.Verify(x => x.UpdatePerson(It.IsAny<Person>()), Times.Once);
            //notificationService.Verify(x => x.DissociateNotificationContacts(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DisableXMatterProfile_Calls_NotificationService_DissociateNotificationContactsMethod()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.GetXMPersonGuid(It.IsAny<string>())).Returns(Guid.NewGuid().ToString());
            notificationService.Setup(x => x.DissociateNotificationContacts(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            Person person = GetFakePersonObject();
            xMatterPersonService.Setup(x => x.UpdatePerson(It.IsAny<Person>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, notificationService.Object, null, null);
            personLogic.DisableXMattersProfile("", "");
            notificationService.Verify(x => x.DissociateNotificationContacts(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GetPerson_Calls_PersonService_GetPersonMethod_AndReturns_Person()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            GetPersonResponse person = GetFakePerson();
            xMatterPersonService.Setup(x => x.GetPerson(It.IsAny<string>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            var responsePerson = personLogic.GetPerson("");
            xMatterPersonService.Verify(x => x.GetPerson(It.IsAny<string>()), Times.Once);
            Assert.That(Utilities.AreObjectsEquivalent(responsePerson, person));
        }

        [Test]
        public void GetPerson_Returns_Null()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            GetPersonResponse person = null;
            xMatterPersonService.Setup(x => x.GetPerson(It.IsAny<string>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            var responsePerson = personLogic.GetPerson("");
            Assert.Null(responsePerson);
        }

        [Test]
        public void GetGroups_Calls_Return_GroupMembers()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            GroupMembers group = GetFakePersonGroups();
            xMatterPersonService.Setup(x => x.GetPersonGroups(It.IsAny<string>(), It.IsAny<bool>())).Returns(group);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            var responseGroups = personLogic.GetGroups("", false);
            xMatterPersonService.Verify(x => x.GetPersonGroups(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            Assert.That(Utilities.AreObjectsEquivalent(responseGroups, group));
        }

        [Test]
        public void GetPersonByTargetName_Calls_PersonService_GetPersonMethod_AndReturns_Person()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            GetPersonResponse person = GetFakePerson();
            xMatterPersonService.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            var responsePerson = personLogic.GetPersonByTargetName("test");
            xMatterPersonService.Verify(x => x.GetPersonByTargetName(It.IsAny<string>()), Times.Once);
            Assert.That(Utilities.AreObjectsEquivalent(responsePerson, person));
        }

        [Test]
        public void GetPersonByTargetName_Set_Event_Calls_PersonService_GetPersonMethod_AndReturns_Person()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            GetPersonResponse person = GetFakePerson();
            xMatterPersonService.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            personLogic.OnPersonDataSaved += OnPersonDataSaved;
            var responsePerson = personLogic.GetPersonByTargetName("test");
            xMatterPersonService.Verify(x => x.GetPersonByTargetName(It.IsAny<string>()), Times.Once);
            Assert.That(Utilities.AreObjectsEquivalent(responsePerson, person));
        }

        [Test]
        public void GetPersonByTargetName_Returns_Null()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            GetPersonResponse person = null;
            xMatterPersonService.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            var responsePerson = personLogic.GetPersonByTargetName("");
            Assert.Null(responsePerson);
        }

        [Test]
        public void GetPersonByTargetName_Set_Event_Returns_Null()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            GetPersonResponse person = null;
            xMatterPersonService.Setup(x => x.GetPersonByTargetName(It.IsAny<string>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            personLogic.OnPersonDataSaved += OnPersonDataSaved;
            var responsePerson = personLogic.GetPersonByTargetName("");
            Assert.Null(responsePerson);
        }

        [Test]
        public void GetPersonDevices_Calls_PersonService_GetPersonDevicesMethod_AndReturns_Devices()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            PersonDevices devices = GetFakePersonDevices();
            xMatterPersonService.Setup(x => x.GetPersonDevices(It.IsAny<string>())).Returns(devices);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            var responseDevices = personLogic.GetPersonDevices("");
            xMatterPersonService.Verify(x => x.GetPersonDevices(It.IsAny<string>()), Times.Once);
            Assert.That(Utilities.AreObjectsEquivalent(responseDevices, devices));
        }

        [Test]
        public void GetPersonDevices_Returns_Null()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            PersonDevices devices = null;
            xMatterPersonService.Setup(x => x.GetPersonDevices(It.IsAny<string>())).Returns(devices);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            var responseDevices = personLogic.GetPersonDevices("");
            Assert.Null(responseDevices);
        }

        [Test]
        public void GetSfContact_Calls_NotificationService_GetContactsByAdfsGuidMethod()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            var nArray = new NotificationContact[] { };
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            personLogic.GetSfContact(nArray.ToList());
            notificationService.Verify(x => x.GetContactsByAdfsGuid(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GetSfContact_Calls_List_NotificationContact_Is_Null_Return_Null()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            List<NotificationContact> contacts = null;
            //notificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Returns(nArray);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            var sfcontacts = personLogic.GetSfContact(contacts);
            Assert.IsNull(sfcontacts);
        }

        [Test]
        public void GetSfContact_Returns_PersonObject()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            //notificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Returns(GetNotificationContacts());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            var contacts = GetNotificationContacts();
            var person = personLogic.GetSfContact(contacts.ToList());
            Assert.NotNull(person);
            Assert.AreEqual(person.TargetName, "TestId");
            Assert.AreEqual(person.FirstName, "TestFirstName");
            Assert.AreEqual(person.LastName, "TestLastName");
        }

        [Test]
        public void EnableXMattersProfile_Calls_PersonService_UpdatePersonMethod()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            Person person = GetFakePersonObject();
            xMatterPersonService.Setup(x => x.UpdatePerson(It.IsAny<Person>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            personLogic.EnableXMattersProfile("2a678fe6-27b8-c8e6-82a0-5f78da890cd4");
            xMatterPersonService.Verify(x => x.UpdatePerson(It.IsAny<Person>()), Times.Once);
        }

        [Test]
        public void EnableXMattersProfile_Set_event_Calls_PersonService_UpdatePersonMethod()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            Person person = GetFakePersonObject();
            xMatterPersonService.Setup(x => x.UpdatePerson(It.IsAny<Person>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            personLogic.OnPersonDataSaved += OnPersonDataSaved;
            personLogic.EnableXMattersProfile("2a678fe6-27b8-c8e6-82a0-5f78da890cd4");
            xMatterPersonService.Verify(x => x.UpdatePerson(It.IsAny<Person>()), Times.Once);
        }

        [Test]
        public void RemoveDevice_Calls_PersonService_RemoveDevice()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            DeviceResponse device = Device();
            DeviceRequest d = DeviceRequest();
            xMatterPersonService.Setup(x => x.RemoveDevice(It.IsAny<string>())).Returns(device);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            personLogic.RemoveDevice(d);
            xMatterPersonService.Verify(x => x.RemoveDevice(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void RemoveDevice_Set_Event_Calls_PersonService_RemoveDevice()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            DeviceResponse device = Device();
            DeviceRequest d = DeviceRequest();
            xMatterPersonService.Setup(x => x.RemoveDevice(It.IsAny<string>())).Returns(device);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            personLogic.OnDeviceSaved += OnDeviceDataSaved;
            personLogic.RemoveDevice(d);
            xMatterPersonService.Verify(x => x.RemoveDevice(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void UpdateNotificationProfile_Calls_Exception()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.UpdateNotificationProfile(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);

            Assert.Throws<Exception>(() => personLogic.UpdateNotificationProfile("testId", "testId1"));
        }


        [Test]
        public void UpdateNotificationProfile_Calls_ServiceException()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.UpdateNotificationProfile(It.IsAny<string>(), It.IsAny<string>())).Throws(new EndpointNotFoundException());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            //Assert.Throws<ServiceException>(() => personLogic.UpdateNotificationProfile("testId", "testId1"));
            Assert.That(() => personLogic.UpdateNotificationProfile("testId", "testId1"), Throws.InnerException.TypeOf<EndpointNotFoundException>());
        }

        [Test]
        public void UpdatePersonTimeZone_Calls_PersonService_UpdatePersonMethod_AndReturns_True()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            Person person = GetFakePersonObject();
            xMatterPersonService.Setup(x => x.UpdatePerson(It.IsAny<Person>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            bool returnValue = personLogic.UpdatePersonTimeZone(person);
            xMatterPersonService.Verify(x => x.UpdatePerson(It.IsAny<Person>()), Times.Once);
            Assert.AreEqual(returnValue, true);
        }

        [Test]
        public void UpdatePersonTimeZone_Set_Event_Calls_PersonService_UpdatePersonMethod_AndReturns_True()
        {
            var xMatterPersonService = new Mock<IXMatterPersonService>();
            Person person = GetFakePersonObject();
            xMatterPersonService.Setup(x => x.UpdatePerson(It.IsAny<Person>())).Returns(person);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(xMatterPersonService.Object, null, null, null);
            personLogic.OnPersonDataSaved += OnPersonDataSaved;
            bool returnValue = personLogic.UpdatePersonTimeZone(person);
            xMatterPersonService.Verify(x => x.UpdatePerson(It.IsAny<Person>()), Times.Once);
            Assert.AreEqual(returnValue, true);
        }

        [Test]
        public void UpdatePersonTimeZone_Returns_False_When_PersonNull()
        {
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, null, null, null);
            bool returnValue = personLogic.UpdatePersonTimeZone(null);
            Assert.AreEqual(returnValue, false);
        }

        [Test]
        public void UpdateNotificationProfile_Calls_NotificationService_UpdateNotificationProfileMethod()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.UpdateNotificationProfile(It.IsAny<string>(), It.IsAny<string>()));
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            personLogic.UpdateNotificationProfile("testID", "testID_1");
            notificationService.Verify(x => x.UpdateNotificationProfile("testID", "testID_1"), Times.Once);
        }

        [Test]
        public void AddDeviceToShift_Calls_Shifts_Null()
        {
            var _xmatterGroupService = new Mock<IXMatterGroupService>();
            var groupShifts = GetFakeNullGroupShifts();
            var groupShift = GetFakeGroupShift();
            ShiftResponse shiftResponse = new ShiftResponse();
            _xmatterGroupService.Setup(x => x.GetShift(It.IsAny<string>())).Returns(groupShifts);
            _xmatterGroupService.Setup(x => x.CreateShift(It.IsAny<string>())).Returns(groupShift);
            _xmatterGroupService.Setup(x => x.AddDeviceToShift(It.IsAny<string>(), It.IsAny<string>())).Returns(shiftResponse);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, null, null, _xmatterGroupService.Object);
            personLogic.AddDeviceToShift("testID", "testID_1");
            _xmatterGroupService.Verify(x => x.CreateShift("testID"), Times.Once);
            _xmatterGroupService.Verify(x => x.GetShift("testID"), Times.AtLeastOnce);
            _xmatterGroupService.Verify(x => x.AddDeviceToShift("testID", "testID_1"), Times.Once);
        }

        [Test]
        public void AddDeviceToShift_Calls_Shifts_HasShift()
        {
            var _xmatterGroupService = new Mock<IXMatterGroupService>();
            var groupShifts = GetFakeGroupShifts();
            var groupShift = GetFakeGroupShift();
            ShiftResponse shiftResponse = new ShiftResponse();
            _xmatterGroupService.Setup(x => x.GetShift(It.IsAny<string>())).Returns(groupShifts);
            _xmatterGroupService.Setup(x => x.CreateShift(It.IsAny<string>())).Returns(groupShift);
            _xmatterGroupService.Setup(x => x.AddDeviceToShift(It.IsAny<string>(), It.IsAny<string>())).Returns(shiftResponse);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, null, null, _xmatterGroupService.Object);
            personLogic.AddDeviceToShift("testID", "testID_1");
            _xmatterGroupService.Verify(x => x.CreateShift("testID"), Times.Never);
            _xmatterGroupService.Verify(x => x.GetShift("testID"), Times.Once);
            _xmatterGroupService.Verify(x => x.AddDeviceToShift("testID", "testID_1"), Times.Once);
        }

        [Test]
        public void SavePersonNotificationGroups_WithNoUpdates()
        {
            var oldGroup = GetFakeNotificationGroups1();
            var newGroup = GetFakeNotificationGroups1();
            var groupId = "group1";
            Group group = new Group()
            {
                TargetName = "GroupName1"
            };
            DeviceResponse deviceResponse = new DeviceResponse();
            ShiftResponse shiftResponse = new ShiftResponse();
            var groupShift = GetFakeGroupShift();
            var groupShifts = GetFakeGroupShifts();
            var _xmatterGroupService = new Mock<IXMatterGroupService>();
            _xmatterGroupService.Setup(x => x.GetGroupIdFromName(It.IsAny<string>())).Returns(groupId);
            _xmatterGroupService.Setup(x => x.CreateGroup(It.IsAny<Group>())).Returns(group);
            _xmatterGroupService.Setup(x => x.AddDeviceToGroup(It.IsAny<string>(), It.IsAny<string>())).Returns(deviceResponse);
            _xmatterGroupService.Setup(x => x.GetShift(It.IsAny<string>())).Returns(groupShifts);
            _xmatterGroupService.Setup(x => x.CreateShift(It.IsAny<string>())).Returns(groupShift);
            _xmatterGroupService.Setup(x => x.AddDeviceToShift(It.IsAny<string>(), It.IsAny<string>())).Returns(shiftResponse);

            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, null, null, _xmatterGroupService.Object);
            personLogic.SavePersonNotificationGroups(oldGroup, newGroup);

            _xmatterGroupService.Verify(x => x.GetGroupIdFromName(It.IsAny<string>()), Times.Never);
            _xmatterGroupService.Verify(x => x.CreateGroup(It.IsAny<Group>()), Times.Never);
            _xmatterGroupService.Verify(x => x.AddDeviceToGroup(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _xmatterGroupService.Verify(x => x.GetShift(It.IsAny<string>()), Times.Never);
            _xmatterGroupService.Verify(x => x.CreateShift(It.IsAny<string>()), Times.Never);
            _xmatterGroupService.Verify(x => x.AddDeviceToShift(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void SavePersonNotificationGroups_WithUpdates()
        {
            var oldGroup = GetFakeNotificationGroups1();
            var newGroup = GetFakeNotificationGroups2();
            var groupId = "";
            Group group = new Group()
            {
                TargetName = "GroupName1"
            };
            DeviceResponse deviceResponse = new DeviceResponse();
            ShiftResponse shiftResponse = new ShiftResponse();
            var groupShift = GetFakeGroupShift();
            GroupShifts groupShifts = null;
            var _xmatterGroupService = new Mock<IXMatterGroupService>();
            _xmatterGroupService.Setup(x => x.GetGroupIdFromName(It.IsAny<string>())).Returns(groupId);
            _xmatterGroupService.Setup(x => x.CreateGroup(It.IsAny<Group>())).Returns(group);
            _xmatterGroupService.Setup(x => x.AddDeviceToGroup(It.IsAny<string>(), It.IsAny<string>())).Returns(deviceResponse);
            _xmatterGroupService.Setup(x => x.GetShift(It.IsAny<string>())).Returns(groupShifts);
            _xmatterGroupService.Setup(x => x.CreateShift(It.IsAny<string>())).Returns(groupShift);
            _xmatterGroupService.Setup(x => x.AddDeviceToShift(It.IsAny<string>(), It.IsAny<string>())).Returns(shiftResponse);

            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, null, null, _xmatterGroupService.Object);
            personLogic.SavePersonNotificationGroups(oldGroup, newGroup);

            _xmatterGroupService.Verify(x => x.GetGroupIdFromName(It.IsAny<string>()), Times.AtLeastOnce);
            _xmatterGroupService.Verify(x => x.CreateGroup(It.IsAny<Group>()), Times.AtLeastOnce);
        }

        [Test]
        public void SavePersonNotificationGroups_WithUpdatesThrowsExceptionOnCreateGroup()
        {
            var oldGroup = GetFakeNotificationGroups1();
            var newGroup = GetFakeNotificationGroups2();
            var groupId = "";
            Group group = new Group()
            {
                TargetName = "GroupName1"
            };
            DeviceResponse deviceResponse = new DeviceResponse();
            ShiftResponse shiftResponse = new ShiftResponse();
            var groupShift = GetFakeGroupShift();
            GroupShifts groupShifts = null;
            var _xmatterGroupService = new Mock<IXMatterGroupService>();
            _xmatterGroupService.Setup(x => x.GetGroupIdFromName(It.IsAny<string>())).Returns(groupId);
            _xmatterGroupService.Setup(x => x.CreateGroup(It.IsAny<Group>())).Throws(new Exception());
            _xmatterGroupService.Setup(x => x.AddDeviceToGroup(It.IsAny<string>(), It.IsAny<string>())).Returns(deviceResponse);
            _xmatterGroupService.Setup(x => x.GetShift(It.IsAny<string>())).Returns(groupShifts);
            _xmatterGroupService.Setup(x => x.CreateShift(It.IsAny<string>())).Returns(groupShift);
            _xmatterGroupService.Setup(x => x.AddDeviceToShift(It.IsAny<string>(), It.IsAny<string>())).Returns(shiftResponse);

            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, null, null, _xmatterGroupService.Object);
            personLogic.SavePersonNotificationGroups(oldGroup, newGroup);

            _xmatterGroupService.Verify(x => x.GetGroupIdFromName(It.IsAny<string>()), Times.AtLeastOnce);
            _xmatterGroupService.Verify(x => x.CreateGroup(It.IsAny<Group>()), Times.AtLeastOnce);
        }

        [Test]
        public void UpdateSFContactsFirstnameLastName_successFull()
        {
            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            _sfNotificationService.Setup(x => x.UpdateSFContactsFirstnameLastName("firstname","lastname","guid")).Returns(true);
            var _xMatterPersonService = new Mock<IXMatterPersonService>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            PersonBusinessLogic personLogic = new PersonBusinessLogic(_xMatterPersonService.Object, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            var result = personLogic.UpdateSFContactsFirstnameLastName("firstname", "lastname", "guid");
            _sfNotificationService.Verify(x => x.UpdateSFContactsFirstnameLastName("firstname", "lastname", "guid"), Times.AtLeastOnce);
            Assert.IsTrue(result);
        }

        [Test]
        public void UpdateSFContactsFirstnameLastName_unsuccessFull()
        {
            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            _sfNotificationService.Setup(x => x.UpdateSFContactsFirstnameLastName("firstname", "lastname", "guid")).Returns(false);
            var _xMatterPersonService = new Mock<IXMatterPersonService>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            PersonBusinessLogic personLogic = new PersonBusinessLogic(_xMatterPersonService.Object, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            var result = personLogic.UpdateSFContactsFirstnameLastName("firstname", "lastname", "guid");
            _sfNotificationService.Verify(x => x.UpdateSFContactsFirstnameLastName("firstname", "lastname", "guid"), Times.AtLeastOnce);
            Assert.IsFalse(result);
        }

        [Test]
        public void UpdateSFContactsFirstnameLastName_Throws_Exception_Return_False()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.UpdateSFContactsFirstnameLastName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            var result = personLogic.UpdateSFContactsFirstnameLastName("testId", "testId1", "test");
            Assert.IsFalse(result);
        }

        [Test]
        public void UpdateSFContactsFirstnameLastName_Throws_ServiceException()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.UpdateSFContactsFirstnameLastName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new EndpointNotFoundException());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            //Assert.Throws<EndpointNotFoundException>(() => personLogic.UpdateSFContactsFirstnameLastName("testId", "testId1", "test"));
        }

        #region Private Methods

        private GetPersonResponse getPersonObject()
        {
            return new GetPersonResponse() { Id = "Testid", TargetName = "TestName" };
        }

        private PersonDevices getPersonDevices()
        {
            var devices = new List<DeviceResponse>();
            devices.Add(new DeviceResponse() { Id = "Id1", Name = "Secondary Email" });
            devices.Add(new DeviceResponse() { Id = "Id2", Name = "Work Email" });
            var personDevices = new PersonDevices()
            {
                Devices = devices,
                Count = 2
            };
            return personDevices;
        }

        private Dictionary<string, XMUserDetails> getEventNotifications()
        {
            var eventNotifications = new Dictionary<string, XMUserDetails>();
            var maintenanceNotifications = new Dictionary<string, XMUserDetails>();
            CadeBillAccount[] account1 = new CadeBillAccount[1];
            CadeBillAccount[] account2 = new CadeBillAccount[1];
            CadeBillAccount[] account3 = new CadeBillAccount[1];
            account1[0] = new CadeBillAccount();
            account2[0] = new CadeBillAccount();
            account3[0] = new CadeBillAccount();

            account1[0].CadebillAccountNo__c = "1";
            account2[0].CadebillAccountNo__c = "2";
            account3[0].CadebillAccountNo__c = "3";

            account1[0].Id = "1";
            account2[0].Id = "2";
            account3[0].Id = "3";

            eventNotifications.Add("test1", new XMUserDetails() { accounts = account1, ClusterPrefix = "none", Product = "product1", XmattersGroupName = "Group1" });
            eventNotifications.Add("test2", new XMUserDetails() { accounts = account2, ClusterPrefix = "cluster2", Product = "product2", XmattersGroupName = "Group2" });
            eventNotifications.Add("test3", new XMUserDetails() { accounts = account3, ClusterPrefix = "cluster3", Product = "product3", XmattersGroupName = "Group3" });
            return eventNotifications;
        }

        private Dictionary<string, XMUserDetails> getMaintenanceNotifications()
        {
            var maintenanceNotifications = new Dictionary<string, XMUserDetails>();
            CadeBillAccount[] account1 = new CadeBillAccount[1];
            CadeBillAccount[] account2 = new CadeBillAccount[1];
            CadeBillAccount[] account3 = new CadeBillAccount[1];

            account1[0] = new CadeBillAccount();
            account2[0] = new CadeBillAccount();
            account3[0] = new CadeBillAccount();

            account1[0].CadebillAccountNo__c = "1";
            account2[0].CadebillAccountNo__c = "2";
            account3[0].CadebillAccountNo__c = "3";

            account1[0].Id = "1";
            account2[0].Id = "2";
            account3[0].Id = "3";

            maintenanceNotifications.Add("test1", new XMUserDetails() { accounts = account1, ClusterPrefix = "none", Product = "product1", XmattersGroupName = "Group1" });
            maintenanceNotifications.Add("test2", new XMUserDetails() { accounts = account2, ClusterPrefix = "cluster2", Product = "product2", XmattersGroupName = "Group2" });
            maintenanceNotifications.Add("test3", new XMUserDetails() { accounts = account3, ClusterPrefix = "cluster3", Product = "product3", XmattersGroupName = "Group3" });
            return maintenanceNotifications;
        }

        private GroupMembers getGroupMembers()
        {
            GroupMember member1 = new GroupMember() { Group = new Group() { TargetName = "Group1" }, Member = new Member() { Id = "Id1" } };
            GroupMember member2 = new GroupMember() { Group = new Group() { TargetName = "Group2" }, Member = new Member() { Id = "Id2" } };
            var member = new List<GroupMember>();
            member.Add(member1);
            member.Add(member2);
            return new GroupMembers() { GroupMember = member };
        }

        private DeviceResponse Device() => new DeviceResponse
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Secondary Email",
            EmailAddress = "test@yopmail.com",
            TargetName = "yovanaTest|Secondary Email",
            DeviceType = "EMAIL",
            Description = "test@yopmail.com",
            TestStatus = "UNTESTED",
            ExternallyOwned = false,
            DefaultDevice = false,
            PriorityThreshold = "LOW",
            Sequence = 1,
            Delay = 0,
            Owner = null,
            RecipientType = "DEVICE",
            Status = "ACTIVE"
        };

        private DeviceRequest DeviceRequest() => new DeviceRequest
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Secondary Email",
            EmailAddress = "test@yopmail.com",
            DeviceType = "EMAIL",
            Description = "test@yopmail.com",
            Owner = null,
        };

        private Person GetFakePersonObject()
        {
            return new Person
            {
                Id = Guid.NewGuid(),
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                Status = "ACTIVE",
                RecipientType = "PERSON"
            };
        }

        private GetPersonResponse GetFakePerson()
        {
            return new GetPersonResponse
            {
                Id = Guid.NewGuid().ToString(),
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                Status = "ACTIVE",
                RecipientType = "PERSON",
                Timezone = "US/Pacific"
            };
        }

        private PersonDevices GetFakePersonDevices() => new PersonDevices
        {
            Count = 1,
            Total = 1,
            Devices = new List<DeviceResponse> { Device() }
        };

        private NotificationContact[] GetNotificationContacts() => new NotificationContact[]
        {
           new NotificationContact() { Id = "TestId", FirstName = "TestFirstName", LastName = "TestLastName"}
        };

        private GroupMembers GetFakePersonGroups() => new GroupMembers
        {
            Count = 1,
            Total = 1,
            GroupMember = new List<GroupMember> { GetFakeGroupMember() }

        };
        private GroupMember GetFakeGroupMember() => new GroupMember
        {
            Group = GetFakeGroup(),
            Member = GetFakeMember()
        };
        private Member GetFakeMember()
        {
            return new Member
            {
                Id = Guid.NewGuid().ToString(),
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                RecipientType = "PERSON",
                links = new Links { Self = $"/api/xm/1/people/" },
            };
        }

        private Group GetFakeGroup() => new Group
        {
            Id = Guid.NewGuid().ToString(),
            TargetName = "Our_INC_Test_MikeB",
            RecipientType = "GROUP",
            Links = new Links { Self = $"/api/xm/1/groups/" }
        };

        private GroupShift GetFakeGroupShift() => new GroupShift
        {
            Id = "24x7",
            Name = "24x7"
        };

        private GroupShifts GetFakeGroupShifts() => new GroupShifts
        {
            Count = 1,
            GroupShift = new List<GroupShift> { GetFakeGroupShift() },
            Link = new Links { Self = $"/api/xm/1/groups/{_groupId}/shift" },
            Total = 1
        };

        private GroupShifts GetFakeNullGroupShifts() => new GroupShifts
        {
            Count = 1,
            GroupShift = new List<GroupShift> { new GroupShift() },
            Link = new Links { Self = $"/api/xm/1/groups/{_groupId}/shift" },
            Total = 1
        };

        private List<NotificationGroup> GetFakeNotificationGroups1()
        {
            List<NotificationGroup> notificationGroups = new List<NotificationGroup>();
            List<DeviceNotifications> deviceNotifications = new List<DeviceNotifications>()
            {
                new DeviceNotifications()
                {
                    DeviceId = "deviceId1",
                    DeviceType = "Primary Email",
                    IsEventsSubscribed = true,
                    IsMaintainanceSubscribed = true
                },
            };
            notificationGroups.Add(new NotificationGroup() { AccountBU = "BU1", DeviceNotifications = deviceNotifications, EventsGroupName = "EventGN1", MaintainanceGroupName = "MaintGN1", ProductCluster = "PC1" });
            notificationGroups.Add(new NotificationGroup() { AccountBU = "BU2", DeviceNotifications = deviceNotifications, EventsGroupName = "EventGN2", MaintainanceGroupName = "MaintGN2", ProductCluster = "PC2" });
            notificationGroups.Add(new NotificationGroup() { AccountBU = "BU3", DeviceNotifications = deviceNotifications, EventsGroupName = "EventGN3", MaintainanceGroupName = "MaintGN3", ProductCluster = "PC3" });

            return notificationGroups;
        }

        private List<NotificationGroup> GetFakeNotificationGroups2()
        {
            List<NotificationGroup> notificationGroups = new List<NotificationGroup>();
            List<DeviceNotifications> deviceNotifications1 = new List<DeviceNotifications>()
            {
                new DeviceNotifications()
                {
                    DeviceId = "deviceId1",
                    DeviceType = "Secondary Email",
                    IsEventsSubscribed = true,
                    IsMaintainanceSubscribed = true
                },
                new DeviceNotifications()
                {
                    DeviceId = "deviceId2",
                    DeviceType = "Secondary Email",
                    IsEventsSubscribed = false,
                    IsMaintainanceSubscribed = true
                },
                new DeviceNotifications()
                {
                    DeviceId = "deviceId3",
                    DeviceType = "SMS Phone",
                    IsEventsSubscribed = true,
                    IsMaintainanceSubscribed = false
                }
            };

            List<DeviceNotifications> deviceNotifications2 = new List<DeviceNotifications>()
            {
                new DeviceNotifications()
                {
                    DeviceId = "deviceId1",
                    DeviceType = "Secondary Email",
                    IsEventsSubscribed = true,
                    IsMaintainanceSubscribed = true
                },
                new DeviceNotifications()
                {
                    DeviceId = "deviceId2",
                    DeviceType = "Secondary Email",
                    IsEventsSubscribed = false,
                    IsMaintainanceSubscribed = false
                },
                new DeviceNotifications()
                {
                    DeviceId = "deviceId3",
                    DeviceType = "SMS Phone",
                    IsEventsSubscribed = true,
                    IsMaintainanceSubscribed = false
                }
            };

            List<DeviceNotifications> deviceNotifications3 = new List<DeviceNotifications>()
            {
                new DeviceNotifications()
                {
                    DeviceId = "deviceId1",
                    DeviceType = "Secondary Email",
                    IsEventsSubscribed = true,
                    IsMaintainanceSubscribed = false
                },
                new DeviceNotifications()
                {
                    DeviceId = "deviceId2",
                    DeviceType = "Secondary Email",
                    IsEventsSubscribed = true,
                    IsMaintainanceSubscribed = true
                },
                new DeviceNotifications()
                {
                    DeviceId = "deviceId3",
                    DeviceType = "SMS Phone",
                    IsEventsSubscribed = false,
                    IsMaintainanceSubscribed = false
                }
            };
            notificationGroups.Add(new NotificationGroup() { AccountBU = "BU1", DeviceNotifications = deviceNotifications1, EventsGroupName = "EventGN1", MaintainanceGroupName = "MaintGN1", ProductCluster = "PC1" });
            notificationGroups.Add(new NotificationGroup() { AccountBU = "BU2", DeviceNotifications = deviceNotifications2, EventsGroupName = "EventGN2", MaintainanceGroupName = "MaintGN2", ProductCluster = "PC2" });
            notificationGroups.Add(new NotificationGroup() { AccountBU = "BU3", DeviceNotifications = deviceNotifications3, EventsGroupName = "EventGN1", MaintainanceGroupName = "MaintGN1", ProductCluster = "PC3" });

            return notificationGroups;
        }

        private void OnPersonDataSaved(string type, Person person)
        {
            
        }

        private void OnDeviceDataSaved(string type, DeviceRequest device)
        {
        }

        #endregion
    }
}
