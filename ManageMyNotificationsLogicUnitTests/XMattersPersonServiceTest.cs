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
namespace ManageMyNotificationsLogicUnitTests
{
    /// <summary>
    /// XMattersPersonServiceTest test the XMattersPersonService and all its methods, for this we are using fake data
    /// </summary>

    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class XMattersPersonServiceTest
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

        //[Test, Property("XMatterPersonService", "GetPerson")]
        //public void TestGetPerson()
        //{
        //    Person fakePerson = GetFakePerson();

        //    _mockXMattersServiceHelper.Setup(x =>
        //        x.CallXMatters<object, Person>(It.IsAny<string>(), HttpMethod.Get, null)).Returns(fakePerson);

        //    var person = _xMatterPersonService.GetPerson(_personId.ToString());

        //    Assert.IsNotNull(person);
        //    Assert.AreEqual(_personId, person.Id);
        //    Assert.AreEqual("tester", person.TargetName);
        //    Assert.AreEqual("ACTIVE", person.Status);
        //    Assert.AreEqual("PERSON", person.RecipientType);
        //}

        [Test, Property("XMatterPersonService", "GetPersonDevices")]
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

        //[Test, Property("XMatterPersonService", "GetPersonGroups")]
        //public void TestGetPersonGroups()
        //{
        //    GroupMembers fakePersonGroups = GetFakePersonGroups();
        //    _mockXMattersServiceHelper
        //        .Setup(x => x.CallXMatters<object, GroupMembers>(It.IsAny<string>(), HttpMethod.Get, null))
        //        .Returns(fakePersonGroups);

        //    var personGroups = _xMatterPersonService.GetPersonGroups(_personId.ToString());

        //    Assert.IsNotNull(personGroups);
        //    Assert.AreEqual(1, personGroups.Count);
        //    Assert.AreEqual(_groupId, personGroups.GroupMember.FirstOrDefault()?.Group.Id);
        //    Assert.AreEqual("Our_INC_Test_MikeB", personGroups.GroupMember.FirstOrDefault()?.Group.TargetName);
        //    Assert.AreEqual("GROUP", personGroups.GroupMember.FirstOrDefault()?.Group.RecipientType);
        //    Assert.AreEqual(_personId, personGroups.GroupMember.FirstOrDefault()?.Member.Id);
        //    Assert.AreEqual("tester", personGroups.GroupMember.FirstOrDefault()?.Member.TargetName);
        //}

        [Test, Property("XMatterPersonService", "CreatePerson")]
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

        [Test, Property("XMatterPersonService", "CreatePerson")]
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

        #region Fake data
        private Person GetFakePerson()
        {
            return new Person
            {
                Id = _personId,
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                Status = "ACTIVE",
                RecipientType = "PERSON",
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
