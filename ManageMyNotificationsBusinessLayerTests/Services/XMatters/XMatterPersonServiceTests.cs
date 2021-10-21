using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Interfaces.XMatters;
using ManageMyNotificationsBusinessLayer.Services.XMatters;
using Moq;
using NUnit.Framework;

namespace ManageMyNotificationsBusinessLayerTests.Services.XMatters
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class XMatterPersonServiceTests
    {
        private readonly Guid _personId = Guid.NewGuid();
        private readonly Guid _groupId = Guid.NewGuid();
        private Mock<IXMattersServiceHelper> _mockXMattersServiceHelper;
        private XMatterPersonService _xMatterPersonService;

        [SetUp]
        public void SetUp()
        {
            _mockXMattersServiceHelper = new Mock<IXMattersServiceHelper>();
            _xMatterPersonService = new XMatterPersonService(_mockXMattersServiceHelper.Object);
        }

        [Test]
        public void TestGetPerson()
        {
            GetPersonResponse fakePerson = GetFakePerson();

            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<object, GetPersonResponse>(It.IsAny<string>(), HttpMethod.Get, null)).ReturnsAsync(fakePerson);

            var person = _xMatterPersonService.GetPerson(_personId.ToString());

            Assert.IsNotNull(person);
            Assert.AreEqual(_personId.ToString(), person.Id);
            Assert.AreEqual("tester", person.TargetName);
            Assert.AreEqual("ACTIVE", person.Status);
            Assert.AreEqual("PERSON", person.RecipientType);
        }

        [Test]
        public void TestGetPersonByTargetName()
        {
            GetPersonResponse fakePerson = GetFakePerson();

            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<object, GetPersonResponse>(It.IsAny<string>(), HttpMethod.Get, null)).ReturnsAsync(fakePerson);

            var person = _xMatterPersonService.GetPersonByTargetName(_personId.ToString());

            Assert.IsNotNull(person);
            Assert.AreEqual(_personId.ToString(), person.Id);
            Assert.AreEqual("tester", person.TargetName);
            Assert.AreEqual("ACTIVE", person.Status);
            Assert.AreEqual("PERSON", person.RecipientType);
        }

        [Test]
        public void TestGetPersonDevices()
        {
            PersonDevices fakeGetPersonDevices = GetFakePersonDevices();
            _mockXMattersServiceHelper
                .Setup(x => x.CallXMatters<object, PersonDevices>(It.IsAny<string>(), HttpMethod.Get, null))
                .ReturnsAsync(fakeGetPersonDevices);
            var personDevices = _xMatterPersonService.GetPersonDevices(_personId.ToString());

            Assert.IsNotNull(personDevices);
            Assert.AreEqual(1, personDevices.Count);
            Assert.AreEqual("Secondary Email", personDevices.Devices.FirstOrDefault()?.Name);
            Assert.AreEqual("EMAIL", personDevices.Devices.FirstOrDefault()?.DeviceType);
            Assert.AreEqual("DEVICE", personDevices.Devices.FirstOrDefault()?.RecipientType);
        }

        [Test]
        public void TestGetPersonGroups_ReturnAll_True()
        {
            GroupMembers fakePersonGroups = GetFakePersonGroups();
            fakePersonGroups.links = new Links(){ Next = "people/group-memberships" };
            GroupMembers secondFakepersonGroups = GetFakePersonGroups();
            secondFakepersonGroups.links = null;
            _mockXMattersServiceHelper
                .SetupSequence(x => x.CallXMatters<object, GroupMembers>(It.IsAny<string>(), HttpMethod.Get, null))
                .ReturnsAsync(fakePersonGroups)
                .ReturnsAsync(secondFakepersonGroups);


            var personGroups = _xMatterPersonService.GetPersonGroups(_personId.ToString(), true);

            Assert.IsNotNull(personGroups);
            Assert.AreEqual(2, personGroups.Count);
            Assert.AreEqual(_groupId.ToString(), personGroups.GroupMember.FirstOrDefault()?.Group.Id);
            Assert.AreEqual("Our_INC_Test_MikeB", personGroups.GroupMember.FirstOrDefault()?.Group.TargetName);
            Assert.AreEqual("GROUP", personGroups.GroupMember.FirstOrDefault()?.Group.RecipientType);
            Assert.AreEqual(_personId.ToString(), personGroups.GroupMember.FirstOrDefault()?.Member.Id);
            Assert.AreEqual("tester", personGroups.GroupMember.FirstOrDefault()?.Member.TargetName);
        }

        [Test]
        public void TestGetPersonGroups_ReturnAll_False()
        {
            GroupMembers fakePersonGroups = GetFakePersonGroups();
            fakePersonGroups.links = new Links() { Next = "people/group-memberships" };
            GroupMembers secondFakepersonGroups = GetFakePersonGroups();
            secondFakepersonGroups.links = null;
            _mockXMattersServiceHelper
                .SetupSequence(x => x.CallXMatters<object, GroupMembers>(It.IsAny<string>(), HttpMethod.Get, null))
                .ReturnsAsync(fakePersonGroups)
                .ReturnsAsync(secondFakepersonGroups);


            var personGroups = _xMatterPersonService.GetPersonGroups(_personId.ToString(), false);

            Assert.IsNotNull(personGroups);
            Assert.AreEqual(1, personGroups.Count);
            Assert.AreEqual(_groupId.ToString(), personGroups.GroupMember.FirstOrDefault()?.Group.Id);
            Assert.AreEqual("Our_INC_Test_MikeB", personGroups.GroupMember.FirstOrDefault()?.Group.TargetName);
            Assert.AreEqual("GROUP", personGroups.GroupMember.FirstOrDefault()?.Group.RecipientType);
            Assert.AreEqual(_personId.ToString(), personGroups.GroupMember.FirstOrDefault()?.Member.Id);
            Assert.AreEqual("tester", personGroups.GroupMember.FirstOrDefault()?.Member.TargetName);
        }

        [Test]
        public void TestCreatePerson()
        {
            var xmattersId = Guid.NewGuid();

            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<Person, Person>(It.IsAny<string>(), HttpMethod.Post, It.IsAny<Person>())).ReturnsAsync(new Person
                {
                    Id = xmattersId,
                    FirstName = "AnotherTester",
                    LastName = "Tester",
                    Status = "ACTIVE"
                });

            var person = new Person
            {
                Id = xmattersId,
                FirstName = "AnotherTester",
                LastName = "Teter",
                Status = "ACTIVE"
            };

            var personCreated = _xMatterPersonService.CreatePerson(person);
            Assert.IsNotNull(personCreated);
            Assert.IsNotNull(personCreated.Id);
            Assert.AreEqual("ACTIVE", personCreated.Status);
        }

        [Test]
        public void TestUpdatePerson()
        {
            var xmattersId = Guid.NewGuid();

            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<Person, Person>(It.IsAny<string>(), HttpMethod.Post, It.IsAny<Person>())).ReturnsAsync(new Person
                {
                    Id = xmattersId,
                    Status = "ACTIVE"
                });

            var personToUpdate = new Person
            {
                Id = xmattersId,
                Status = "ACTIVE"
            };

            var personUpdated = _xMatterPersonService.UpdatePerson(personToUpdate);
            Assert.IsNotNull(personUpdated);
            Assert.IsNotNull(personUpdated.Id);
            Assert.AreEqual("ACTIVE", personUpdated.Status);
        }

        [Test]
        public void XMatterPersonService_RemoveDevice()
        {
            DeviceResponse deviceResponse = Device();
            _mockXMattersServiceHelper
                .Setup(x => x.CallXMatters<DeviceRequest, DeviceResponse>(It.IsAny<string>(), HttpMethod.Delete, null))
                .ReturnsAsync(deviceResponse);
            var deviceRemoved = _xMatterPersonService.RemoveDevice(deviceResponse.Id);
            Assert.AreEqual(deviceResponse, deviceRemoved);
        }

        [Test]
        public void XMatterPersonService_CreateDevice_Calls_XMatterServiceHelper_AndReturns_DeviceResponse()
        {
            DeviceRequest deviceRequest = DeviceRequest();
            DeviceResponse deviceResponse = Device();
            _mockXMattersServiceHelper
                .Setup(x => x.CallXMatters<DeviceRequest, DeviceResponse>("devices", HttpMethod.Post, deviceRequest))
                .ReturnsAsync(deviceResponse);
            var deviceCreated = _xMatterPersonService.CreateDevice(deviceRequest);
            Assert.IsNotNull(deviceCreated);
            Assert.That(Utilities.AreObjectsEquivalent(deviceCreated, deviceResponse));
        }

        [Test]
        public void XMatterPersonService_AddPersonToGroup_Calls_XMatterServiceHelper_AndReturns_Person()
        {
            Person person = GetFakePersonObject();
            _mockXMattersServiceHelper
                .Setup(x => x.CallXMatters<Object, Person>($"groups/{_groupId.ToString()}/members", HttpMethod.Post, It.IsAny<object>()))
                .ReturnsAsync(person);
            var actualPerson = _xMatterPersonService.AddPersonToGroup(_groupId.ToString(), _personId.ToString());
            Assert.IsNotNull(actualPerson);
            Assert.That(Utilities.AreObjectsEquivalent(person, actualPerson));
        }

        #region Fake data
        private GetPersonResponse GetFakePerson()
        {
            return new GetPersonResponse
            {
                Id = _personId.ToString(),
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                Status = "ACTIVE",
                RecipientType = "PERSON"
            };
        }

        private Person GetFakePersonObject()
        {
            return new Person
            {
                Id = _personId,
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                Status = "ACTIVE",
                RecipientType = "PERSON"
            };
        }
        private Member GetFakeMember()
        {
            return new Member
            {
                Id = _personId.ToString(),
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                RecipientType = "PERSON",
                links = new Links { Self = $"/api/xm/1/people/{_personId}" },
            };
        }

        private Owner GetFakeOwner()
        {
            return new Owner
            {
                Id = _personId.ToString(),
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                RecipientType = "PERSON",
            };

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
            Owner = GetFakeOwner(),
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

        private Group GetFakeGroup() => new Group
        {
            Id = _groupId.ToString(),
            TargetName = "Our_INC_Test_MikeB",
            RecipientType = "GROUP",
            Links = new Links { Self = $"/api/xm/1/groups/{_groupId}" }
        };

        private PersonDevices GetFakePersonDevices() => new PersonDevices
        {
            Count = 1,
            Total = 1,
            Devices = new List<DeviceResponse> { Device() }
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
        #endregion
    }
}
