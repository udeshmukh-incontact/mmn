using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Interfaces.XMatters;
using ManageMyNotificationsBusinessLayer.Services.XMatters;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace ManageMyNotificationsBusinessLayerTests.Services.XMatters
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class XMatterGroupServiceTests
    {

        private readonly Guid _personId = Guid.NewGuid();
        private readonly Guid _groupId = Guid.NewGuid();
        private readonly Guid _shiftId = Guid.NewGuid();
        private Mock<IXMattersServiceHelper> _mockXMattersServiceHelper;
        private XMatterGroupService _xMatterGroupService;


        [SetUp]
        public void SetUp()
        {
            _mockXMattersServiceHelper = new Mock<IXMattersServiceHelper>();
            _xMatterGroupService = new XMatterGroupService(_mockXMattersServiceHelper.Object);
        }

        [Test]
        public void XMatterGroupService_GetGroupIdFromName_Returns_CorrectGroupId()
        {
            var groups = getFakeGroups();
            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<object, Groups>(It.IsAny<string>(), HttpMethod.Get, null)).ReturnsAsync(groups);
            string groupId = _xMatterGroupService.GetGroupIdFromName("Group1");

            Assert.AreEqual(groupId, groups.GroupMember[0].Id);
        }

        [Test]
        public void XMatterGroupService_GetGroupIdFromName_Returns_EmptyGroupId()
        {
            var groups = getFakeGroups();
            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<object, Groups>(It.IsAny<string>(), HttpMethod.Get, null)).ReturnsAsync(groups);
            string groupId = _xMatterGroupService.GetGroupIdFromName("Group2");

            Assert.AreEqual(groupId, string.Empty);
        }

        [Test]
        public void XMatterGroupService_GetGroupMembers_Calls_XMattersServiceHelper_AndReturns_GroupMembers()
        {
            var groupMembers = GetFakeGroupMembers();
            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<object, GroupMembers>($"groups/{_groupId.ToString()}/members", HttpMethod.Get, null)).ReturnsAsync(groupMembers);
            var actualGroupMmbers = _xMatterGroupService.GetGroupMembers(_groupId.ToString());

            Assert.AreEqual(groupMembers, actualGroupMmbers);
            _mockXMattersServiceHelper.Verify(x =>
                x.CallXMatters<object, GroupMembers>($"groups/{_groupId.ToString()}/members", HttpMethod.Get, null), Times.Once);
        }

        [Test]
        public void XMatterGroupService_CreateGroup_Calls_XMattersServiceHelper_AndReturns_CreatedGroup()
        {
            var group = GetFakeGroup();
            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<Group, Group>("groups", HttpMethod.Post, group)).ReturnsAsync(group);
            var actualGroup = _xMatterGroupService.CreateGroup(group);

            Assert.AreEqual(group, actualGroup);
            _mockXMattersServiceHelper.Verify(x =>
                x.CallXMatters<Group, Group>("groups", HttpMethod.Post, group), Times.Once);
        }

        [Test]
        public void XMatterGroupService_AddDeviceToGroup_Calls_XMattersServiceHelper_AndReturns_AddedDevice()
        {
            var device = Device();
            string deviceId = "deviceId1";
            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<object, DeviceResponse>($"groups/{_groupId.ToString()}/members", HttpMethod.Post,
                It.IsAny<object>())).ReturnsAsync(device);
            var actualDevice = _xMatterGroupService.AddDeviceToGroup(_groupId.ToString(), deviceId);

            Assert.AreEqual(device, actualDevice);
            _mockXMattersServiceHelper.Verify(x =>
                x.CallXMatters<object, DeviceResponse>($"groups/{_groupId.ToString()}/members", HttpMethod.Post,
                It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void XMatterGroupService_AddDeviceToShift_Calls_XMattersServiceHelper_AndReturns_AddedDevice()
        {
            var shift = new ShiftResponse();
            string deviceId = "deviceId1";
            string shiftName = "24x7";
            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<object, ShiftResponse>($"groups/{_groupId}/shifts/{shiftName}/members", HttpMethod.Post,
                It.IsAny<object>())).ReturnsAsync(shift);
            var actualDevice = _xMatterGroupService.AddDeviceToShift(_groupId.ToString(), deviceId);

            Assert.AreEqual(shift, actualDevice);
            _mockXMattersServiceHelper.Verify(x =>
                x.CallXMatters<object, ShiftResponse>($"groups/{_groupId}/shifts/{shiftName}/members", HttpMethod.Post,
                It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void XMatterGroupService_CreateShift_Calls_XMattersServiceHelper_AndReturns_CreatedShift()
        {
            var shift = new { name = "24x7" };
            var newShift = GetFakeGroupShift();
            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<object, GroupShift>($"groups/{_groupId.ToString()}/shifts", HttpMethod.Post, It.IsAny<object>())).ReturnsAsync(newShift);
            var actualGroup = _xMatterGroupService.CreateShift(_groupId.ToString());

            //Assert.AreEqual(shift, actualGroup);
            _mockXMattersServiceHelper.Verify(x =>
                x.CallXMatters<object, GroupShift>($"groups/{_groupId.ToString()}/shifts", HttpMethod.Post, It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void XMatterGroupService_GetGroupShiftIdFromName_Returns_CorrectGroupShiftId()
        {
            GroupShifts shift = GetFakeGroupShifts();
            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<object, GroupShifts>($"groups/{_groupId.ToString()}/shifts/", HttpMethod.Get, null)).ReturnsAsync(shift);

            var groupShift = _xMatterGroupService.GetShift(_groupId.ToString());

            Assert.AreEqual(groupShift.Count, shift.Count);
        }

        [Test]
        public void XMatterGroupService_RemoveDeviceFromGroup_Calls_XMattersServiceHelper_AndReturns_GroupMember()
        {
            var groupMember = GetFakeGroupMember();
            string deviceId = "deviceId1";
            _mockXMattersServiceHelper.Setup(x =>
                x.CallXMatters<object, GroupMember>($"groups/{_groupId.ToString()}/members/{deviceId}", HttpMethod.Delete, null)).ReturnsAsync(groupMember);
            var actualGroupMember = _xMatterGroupService.RemoveDeviceFromGroup(_groupId.ToString(), deviceId);

            Assert.AreEqual(groupMember, actualGroupMember);
            _mockXMattersServiceHelper.Verify(x =>
                x.CallXMatters<object, GroupMember>($"groups/{_groupId.ToString()}/members/{deviceId}", HttpMethod.Delete, null), Times.Once);
        }


        #region Fake Data
        private Groups getFakeGroups()
        {
            return new Groups()
            {
                Count = 1,
                GroupMember = new List<Group>() { GetFakeGroup() }
            };
        }

        private Group GetFakeGroup() => new Group
        {
            Id = _groupId.ToString(),
            TargetName = "Group1",
            RecipientType = "GROUP",
            Links = new Links { Self = $"/api/xm/1/groups/{_groupId}" }
        };

        private GroupShift GetFakeGroupShift() => new GroupShift
        {
            Name = "shift1",
        };

        private GroupShifts GetFakeGroupShifts() => new GroupShifts
        {
            Count = 1,
            GroupShift = new List<GroupShift> { GetFakeGroupShift() },
            Link = new Links { Self = $"/api/xm/1/groups/{_groupId}/shift" },
            Total = 1
        };

        private GroupMembers GetFakeGroupMembers() => new GroupMembers
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
                Id = _personId.ToString(),
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                RecipientType = "PERSON",
                links = new Links { Self = $"/api/xm/1/people/{_personId}" },
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
            Owner = null,
            RecipientType = "DEVICE",
            Status = "ACTIVE"
        };

        #endregion
    }
}
