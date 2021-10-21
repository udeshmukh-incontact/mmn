using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ManageMyNotificationsBusinessLayer;
using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using ManageMyNotificationsBusinessLayer.Proxy;
using Moq;
using NUnit.Framework;

namespace ManageMyNotificationsBusinessLayerTests
{
    [TestFixture]
    public class SyncSFXMattersProfile
    {
        [Test]
        public void SyncSFXMattersProfile_Calls_No_UpdateContectWithXMPersonId_Called()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.UpdateContactWithXMPersonId(It.IsAny<List<string>>(), It.IsAny<string>())).Returns(true);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            GetPersonResponse person = GetFakePerson();
            NotificationContact[] contacts = GetNotificationContacts();
            contacts[0].XMPersonId = "test";
            personLogic.SyncSFXMattersProfile(contacts.ToList(), person);
            notificationService.Verify(x => x.UpdateContactWithXMPersonId(It.IsAny<List<string>>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void SyncSFXMattersProfile_Calls_UpdateContectWithXMPersonId_Called()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.UpdateContactWithXMPersonId(It.IsAny<List<string>>(), It.IsAny<string>())).Returns(true);
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            GetPersonResponse person = GetFakePerson();
            List<NotificationContact> contacts = GetNotification2Contacts();
            contacts[0].XMPersonId = "test";
            personLogic.SyncSFXMattersProfile(contacts, person);
            notificationService.Verify(x => x.UpdateContactWithXMPersonId(It.IsAny<List<string>>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SyncSFXMattersProfile_Calls_Throws_ServiceException()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.UpdateContactWithXMPersonId(It.IsAny<List<string>>(), It.IsAny<string>())).Throws(new Exception("",new EndpointNotFoundException()));
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            GetPersonResponse person = GetFakePerson();
            List<NotificationContact> contacts = GetNotification2Contacts();
            contacts[0].XMPersonId = "test";
            Assert.Throws<ServiceException>(() => personLogic.SyncSFXMattersProfile(contacts, person));
        }

        [Test]
        public void SyncSFXMattersProfile_Calls_Throws_Exception()
        {
            var notificationService = new Mock<ISalesforceNotificationServiceProxy>();
            notificationService.Setup(x => x.UpdateContactWithXMPersonId(It.IsAny<List<string>>(), It.IsAny<string>())).Throws(new Exception());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, notificationService.Object, null, null);
            GetPersonResponse person = GetFakePerson();
            List<NotificationContact> contacts = GetNotification2Contacts();
            contacts[0].XMPersonId = "test";
            personLogic.SyncSFXMattersProfile(contacts, person);
            notificationService.Verify(x => x.UpdateContactWithXMPersonId(It.IsAny<List<string>>(), It.IsAny<string>()), Times.Once);
        }

        #region Private Methods


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


        private NotificationContact[] GetNotificationContacts() => new NotificationContact[]
        {
           new NotificationContact() { Id = "TestId", FirstName = "TestFirstName", LastName = "TestLastName"}
        };

        private List<NotificationContact> GetNotification2Contacts() => new List<NotificationContact>
        {
           new NotificationContact() { Id = "TestId1", FirstName = "TestFirstName", LastName = "TestLastName", XMPersonGuid = "test"},
           new NotificationContact() { Id = "TestId2", FirstName = "TestFirstName", LastName = "TestLastName", XMPersonGuid = ""}
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

        #endregion
    }
}
