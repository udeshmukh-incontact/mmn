using ManageMyNotificationsBusinessLayer;
using ManageMyNotificationsBusinessLayer.inContactSalesforceService;
using ManageMyNotificationsBusinessLayer.Proxy;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ManageMyNotificationsBusinessLayerTests
{
    [TestFixture]
    public class GetSFNotificationProfileTest
    {
        [Test]
        public void Success_One_Contact_Test()
        {
            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _sfNotificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Returns(GetNotification1Contacts().ToArray());
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(getEventNotifications());
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(getMaintenanceNotifications());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            var profile = personLogic.GetSFNotificationProfile("test");

            Assert.AreEqual("test", profile.XMGuid);
            _sfNotificationService.Verify(x => x.GetContactsByAdfsGuid(It.IsAny<string>()), Times.Once);
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType"), Times.Once);
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType"), Times.Once);
        }

        [Test]
        public void Success_WithoutXMGuid_Test()
        {
            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _sfNotificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Returns(GetNotificationWithoutXMProfileContacts().ToArray());
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(getEventNotifications());
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(getMaintenanceNotifications());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            var profile = personLogic.GetSFNotificationProfile("test");

            Assert.IsNotNull(profile.XMGuid);
            Assert.IsEmpty(profile.XMGuid);
            _sfNotificationService.Verify(x => x.GetContactsByAdfsGuid(It.IsAny<string>()), Times.Once);
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType"), Times.Once);
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType"), Times.Once);
        }

        [Test]
        public void Success_One_Contact_Multiple_OneWithoutXMGuid_Test()
        {
            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _sfNotificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Returns(GetNotification2Contacts().ToArray());
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(getEventNotifications());
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(getMaintenanceNotifications());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            var profile = personLogic.GetSFNotificationProfile("test");

            Assert.AreEqual("test", profile.XMGuid);
            _sfNotificationService.Verify(x => x.GetContactsByAdfsGuid(It.IsAny<string>()), Times.Once);
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType"), Times.Exactly(2));
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType"), Times.Exactly(2));
        }

        [Test]
        public void Throws_ServiceException_Test()
        {
            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _sfNotificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Throws(new EndpointNotFoundException());
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(getEventNotifications());
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(getMaintenanceNotifications());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            //Assert.Throws<ServiceException>(() => personLogic.GetSFNotificationProfile("test"));
            Assert.Throws<Exception>(() => personLogic.GetSFNotificationProfile("test"));
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType"), Times.Never);
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType"), Times.Never);
        }

        [Test]
        public void Throws_Exception_Test()
        {
            var _sfNotificationService = new Mock<ISalesforceNotificationServiceProxy>();
            var _xaMatterIntegrationService = new Mock<IXMattersIntegrationServiceProxy>();
            _sfNotificationService.Setup(x => x.GetContactsByAdfsGuid(It.IsAny<string>())).Throws(new FormatException("Invalid Details"));
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType")).Returns(getEventNotifications());
            _xaMatterIntegrationService.Setup(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType")).Returns(getMaintenanceNotifications());
            PersonBusinessLogic personLogic = new PersonBusinessLogic(null, _sfNotificationService.Object, _xaMatterIntegrationService.Object, null);
            Assert.Throws<Exception>(() => personLogic.GetSFNotificationProfile("test"));

            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "eventType"), Times.Never);
            _xaMatterIntegrationService.Verify(x => x.GetXMProfileDetails(It.IsAny<string>(), "MaintType"), Times.Never);
        }

        #region Private Methods

        private List<NotificationContact> GetNotificationWithoutXMProfileContacts() => new List<NotificationContact>
        {
           new NotificationContact() { Id = "TestId", FirstName = "TestFirstName", LastName = "TestLastName", XMPersonGuid = ""}
        };
        private List<NotificationContact> GetNotification1Contacts() => new List<NotificationContact>
        {
           new NotificationContact() { Id = "TestId", FirstName = "TestFirstName", LastName = "TestLastName", XMPersonGuid = "test"}
        };

        private List<NotificationContact> GetNotification2Contacts() => new List<NotificationContact>
        {
           new NotificationContact() { Id = "TestId1", FirstName = "TestFirstName", LastName = "TestLastName", XMPersonGuid = "test"},
           new NotificationContact() { Id = "TestId2", FirstName = "TestFirstName", LastName = "TestLastName", XMPersonGuid = ""}
        };

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

            eventNotifications.Add("test1", new XMUserDetails() { accounts = account1, ClusterPrefix = "cluster1", Product = "product1", XmattersGroupName = "Group1" });
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

            maintenanceNotifications.Add("test1", new XMUserDetails() { accounts = account1, ClusterPrefix = "cluster1", Product = "product1", XmattersGroupName = "Group1" });
            maintenanceNotifications.Add("test2", new XMUserDetails() { accounts = account2, ClusterPrefix = "cluster2", Product = "product2", XmattersGroupName = "Group2" });
            maintenanceNotifications.Add("test3", new XMUserDetails() { accounts = account3, ClusterPrefix = "cluster3", Product = "product3", XmattersGroupName = "Group3" });
            return maintenanceNotifications;
        }

        #endregion
    }
}
