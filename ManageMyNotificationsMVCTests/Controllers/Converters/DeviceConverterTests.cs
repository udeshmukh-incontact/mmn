using System;
using System.Collections.Generic;
using NUnit.Framework;
using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsMVC.Models;
using ManageMyNotificationsMVC.Controllers.Converters;
using System.Diagnostics.CodeAnalysis;

namespace ManageMyNotificationsMVCTests.Controllers.Converters
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class DeviceConverterTests
    {
        private readonly Guid _personId = Guid.NewGuid();
        private readonly Guid _deviceId = Guid.NewGuid();
        private readonly Guid _groupId = Guid.NewGuid();

        [Test]
        public void DeviceConverter_ToViewModel_ConvertsProperly()
        {
            var expected = new NotificationManagerViewModel
            {
                Count = 1,
                Total = 1,
                Person = GetFakeModelPersons(),
                Devices = new List<Device>() { GetFakeEmailDevice() },
                NotificationGroups = new List<NotificationGroup>() { GetFakeNotificationgroups() }
            };

            var personDevices = new PersonDevices()
            {
                Count = 1,
                Total = 1,
                Person = GetFakeBusinessPerson(),
                Devices = new List<DeviceResponse>() { GetFakeDeviceResp() },
                NotificationGroups = new List<NotificationGroup>() { GetFakeNotificationgroups() }
            };

            var actual = personDevices.ToViewModel();
            Assert.AreEqual(actual.Devices.Count, expected.Devices.Count);
            if (actual.Devices.Count > 0 && expected.Devices.Count > 0)
            {
                Assert.AreEqual(actual.Devices[0].Id, expected.Devices[0].Id);
                Assert.IsNull(actual.Devices[0].Status);
                Assert.AreEqual(actual.Devices[0].DeviceTimeFrame[0].DurationInMinutes, 1440);
            }
        }

        [Test]
        public void DeviceConverter_ToViewModel_NULL_ConvertsProperly()
        {
            var expected = new NotificationManagerViewModel
            {
                Count = 1,
                Total = 1,
                Person = GetFakeModelPersons(),
                Devices = new List<Device>() { GetFakeNullTimeFrameDevice() },
                NotificationGroups = new List<NotificationGroup>() { GetFakeNotificationgroups() }
            };

            var personDevices = new PersonDevices()
            {
                Count = 1,
                Total = 1,
                Person = GetFakeBusinessPerson(),
                Devices = new List<DeviceResponse>() { GetFakeNullTimeFrameDeviceResp() },
                NotificationGroups = new List<NotificationGroup>() { GetFakeNotificationgroups() }
            };

            var actual = personDevices.ToViewModel();
            Assert.AreEqual(actual.Devices.Count, expected.Devices.Count);
            if (actual.Devices.Count > 0 && expected.Devices.Count > 0)
            {
                Assert.AreEqual(actual.Devices[0].Id, expected.Devices[0].Id);
                Assert.IsNull(actual.Devices[0].DeviceTimeFrame);
            }
        }

        [Test]
        public void DeviceConverter_ToDomainModel_ForNull()
        {
            var expecteddomain = new PersonDevices()
            {
                Count = 1,
                Total = 1,
                Person = GetFakeBusinessPerson(),
                Devices = null,
                NotificationGroups = new List<NotificationGroup>() { GetFakeNotificationgroups() }
            };

            expecteddomain.Person.Id = null;
            expecteddomain.Person.Timezone = null;

            var notificationManagerViewModel = new NotificationManagerViewModel
            {
                Count = 1,
                Total = 1,
                Person = GetFakeModelPersons(),
                Devices = null,
                NotificationGroups = new List<NotificationGroup>() { GetFakeNotificationgroups() }
            };
            notificationManagerViewModel.Person.Id = null;
            notificationManagerViewModel.Person.TimeZone = null;

            var actual = notificationManagerViewModel.ToDomainModel();
            Assert.IsNull(actual.Person.Id);
            Assert.IsNull(actual.Person.Timezone);
            Assert.AreEqual(0, actual.Devices.Count);
        }

        [Test]
        public void DeviceConverter_ToDomainModel_ConvertsProperly()
        {
            var expecteddomain = new PersonDevices()
            {
                Count = 1,
                Total = 1,
                Person = GetFakeBusinessPerson(),
                Devices = new List<DeviceResponse>() { GetFakeDeviceResp() },
                NotificationGroups = new List<NotificationGroup>() { GetFakeNotificationgroups() }
            };

            var notificationManagerViewModel = new NotificationManagerViewModel
            {
                Count = 1,
                Total = 1,
                Person = GetFakeModelPersons(),
                Devices = new List<Device>() { GetFakeEmailDevice() },
                NotificationGroups = new List<NotificationGroup>() { GetFakeNotificationgroups() }
            };

            var actual = notificationManagerViewModel.ToDomainModel();
            Assert.That(Utilities.AreObjectsEquivalent(actual.Person.Id, expecteddomain.Person.Id));
            Assert.AreEqual(expecteddomain.Count, notificationManagerViewModel.Count);
            Assert.AreEqual(expecteddomain.Total, notificationManagerViewModel.Total);
        }

        [Test]
        public void DeviceConverter_IsEquals_Email_DeviceResponse_Test()
        {
            var deviceResp1 = GetFakeDeviceResp("EMAIL");
            var deviceResp2 = GetFakeDeviceResp("EMAIL");
            var actual = deviceResp1.IsEquals(deviceResp2);
            Assert.IsTrue(actual);
        }
        [Test]
        public void DeviceConverter_IsEquals_TEXT_PHONE_DeviceResponse_Test()
        {
            var deviceResp1 = GetFakeDeviceResp("TEXT_PHONE");
            var deviceResp2 = GetFakeDeviceResp("TEXT_PHONE");
            var actual = deviceResp1.IsEquals(deviceResp2);
            Assert.IsTrue(actual);
        }
        [Test]
        public void DeviceConverter_IsEquals_False_DeviceResponse_Test()
        {
            var deviceResp1 = GetFakeDeviceResp("EMAIL");
            var deviceResp2 = GetFakeDeviceResp("TEXT_PHONE");
            var actual1 = deviceResp1.IsEquals(deviceResp2);
            Assert.IsFalse(actual1);

            var deviceResp3 = GetFakeDeviceResp("TEXT_PHONE");
            var deviceResp4 = GetFakeDeviceResp("EMAIL");
            var actual2 = deviceResp3.IsEquals(deviceResp4);
            Assert.IsFalse(actual2);
        }
        [Test]
        public void DeviceConverter_IsEquals_Istrue_TimeFramedetails_Test()
        {
            var timeFrameDetails1 = GetFakeTimeFrameDetails("24x7x365", "00:00", 1336, false);
            var timeFrameDetails2 = GetFakeTimeFrameDetails("24x7x365", "00:00", 1336, false);

            var actual = timeFrameDetails1.IsEquals(timeFrameDetails2);
            Assert.IsTrue(actual);
        }
        [Test]
        public void DeviceConverter_IsEquals_Isfalse_TimeFramedetails_Test()
        {
            var timeFrameDetails1 = GetFakeTimeFrameDetails("", "", 1, true);
            var timeFrameDetails2 = GetFakeTimeFrameDetails("24x7x365", "00:00", 1336, false);

            var actual = timeFrameDetails1.IsEquals(timeFrameDetails2);
            Assert.IsFalse(actual);
        }

        [Test]
        public void DeviceConverter_NULL_ToViewModel_ConvertsProperly()
        {
            PersonDevices personDevices = null;

            var actual = personDevices.ToViewModel();

            Assert.IsNull(actual);
        }

        [Test]
        public void DeviceConverter_ToDomainModel_Null()
        {
            NotificationManagerViewModel personDevices = null;

            var actual = personDevices.ToDomainModel();

            Assert.IsNull(actual);
        }

        [Test]
        public void DeviceConverter_IsNotEquals_Istrue_TimeFramedetails_Test()
        {
            var timeFrameDetails1 = GetFakeTimeFrameDetails("24x7x365", "00:00", 1336, false);
            var timeFrameDetails2 = GetFakeTimeFrameDetails("24x7", "00:00", 1336, false);

            var actual = timeFrameDetails1.IsEquals(timeFrameDetails2);
            Assert.IsFalse(actual);
        }

        [Test]
        public void DeviceConverter_IsNotEquals_Phone_DeviceResponse_Test()
        {
            var deviceResp1 = GetFakeDeviceResp("TEXT_PHONE", "SMS Phone", "IN");
            var deviceResp2 = GetFakeDeviceResp("TEXT_PHONE", "SMS Phone", "US");
            var actual = deviceResp1.IsEquals(deviceResp2);
            Assert.IsFalse(actual);
        }

        [Test]
        public void DeviceConverter_IsNotEquals_Email_DeviceResponse_Test()
        {
            var deviceResp1 = GetFakeDeviceResp("EMAIL", "Primary Phone");
            var deviceResp2 = GetFakeDeviceResp("EMAIL", "Secondary Phone");
            var actual = deviceResp1.IsEquals(deviceResp2);
            Assert.IsFalse(actual);
        }

        [Test]
        public void DeviceConverter_IsNotEquals_TimeFrame_Count_Test()
        {
            List<TimeFramedetails> timeframe1 = new List<TimeFramedetails>() { GetFakeTimeFrameDetails("24x7x365", "00:00", 1336, false) };
            List<TimeFramedetails> timeframe2 = new List<TimeFramedetails>() {
                GetFakeTimeFrameDetails("24x7x365", "00:00", 1336, false),
                GetFakeTimeFrameDetails("24x7", "00:01", 1337, false)
            };
            var actual = timeframe1.IsEquals(timeframe2);
            Assert.IsFalse(actual);
        }

        [Test]
        public void DeviceConverter_IsNotEquals_TimeFrame_Valus_Test()
        {
            List<TimeFramedetails> timeframe1 = new List<TimeFramedetails>() {
                GetFakeTimeFrameDetails("24x7x365", "00:00", 1336, false),
                GetFakeTimeFrameDetails("24x7", "00:02", 1337, false)};

            List<TimeFramedetails> timeframe2 = new List<TimeFramedetails>() {
                GetFakeTimeFrameDetails("24x7x365", "00:00", 1336, false),
                GetFakeTimeFrameDetails("24x7", "00:01", 1337, false)
            };
            var actual = timeframe1.IsEquals(timeframe2);
            Assert.IsFalse(actual);
        }

        [Test]
        public void DeviceConverter_NotEquals_Device_DeviceResponse_Test()
        {
            var deviceResp1 = GetFakeDeviceResp("EMAIL");
            var deviceResp2 = GetFakeDeviceResp("TEXT_PHONE");
            var actual = deviceResp1.IsEquals(deviceResp2);
            Assert.IsFalse(actual);
        }

        #region Fake data
        private Person GetFakeBusinessPerson()
        {
            return new Person
            {
                Id = _personId,
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                Status = "ACTIVE",
                Timezone = "IST",
            };
        }
        private Persons GetFakeModelPersons()
        {
            return new Persons
            {
                Id = _personId.ToString(),
                TargetName = "tester",
                FirstName = "Test",
                LastName = "User",
                Status = "ACTIVE",
                TimeZone = "IST",
            };
        }
        private DeviceResponse GetFakeDeviceResp() => new DeviceResponse
        {
            Id = _deviceId.ToString(),
            Name = "Secondary Email",
            EmailAddress = "test@yopmail.com",
            TargetName = "yovanaTest|Secondary Email",
            DeviceType = "EMAIL",
            Description = "test@yopmail.com",
            TimeFrames = GetFakeTimeFrameList(),
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
        private DeviceResponse GetFakeNullTimeFrameDeviceResp() => new DeviceResponse
        {
            Id = _deviceId.ToString(),
            Name = "Secondary Email",
            EmailAddress = "test@yopmail.com",
            TargetName = "yovanaTest|Secondary Email",
            DeviceType = "EMAIL",
            Description = "test@yopmail.com",
            TimeFrames = null,
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
        private DeviceResponse GetFakeDeviceResp(string deviceType = "", string name = "", string country = "") => new DeviceResponse
        {
            Id = _deviceId.ToString(),
            Name = name,
            Country = country,
            Description = "",
            PhoneNumber = "",
            TimeFrames = GetFakeTimeFrames(),
            DeviceType = deviceType,
            TestStatus = "UNTESTED",
            Status = "ACTIVE"
        };
        private Device GetFakeEmailDevice() => new Device
        {
            Id = _deviceId.ToString(),
            Name = "Secondary Email",
            EmailAddress = "test@yopmail.com",
            DeviceType = "EMAIL",
            Description = "test@yopmail.com",
            Status = "ACTIVE",
            PhoneNumber = "",
            DeviceTimeFrame = GetDefaultFakeTimeFrame(),
            CountryCode = ""
        };

        private Device GetFakeNullTimeFrameDevice() => new Device
        {
            Id = _deviceId.ToString(),
            Name = "SMS Phone",
            EmailAddress = "",
            DeviceType = "",
            Description = "+911234567890",
            Status = "ACTIVE",
            PhoneNumber = "1234567890",
            DeviceTimeFrame = null,
            CountryCode = ""
        };
        private List<TimeFrame> GetDefaultFakeTimeFrame() => new List<TimeFrame>()
        {
            new TimeFrame()
            {
                Name = "24x7x365",
                StartTime = "00:00",
                EndTime = "00:00",
                DurationInMinutes = 1440,
                Days = new List<string> { "MO", "TU", "WE", "TH", "FR", "SA", "SU" },
                ExcludeHolidays = false
            }
        };

        private Timeframe GetFakeTimeFrameList()
        {
            var timeframe = new Timeframe();
            var timeFramedetails = new TimeFramedetails()
            {
                name = "24x7x365",
                startTime = "00:00",
                durationInMinutes = 1440,
                days = new List<string> { "MO", "TU", "WE", "TH", "FR", "SA", "SU" },
                excludeHolidays = false
            };
            timeframe.count = 1;
            timeframe.data = new List<TimeFramedetails>();
            timeframe.data.Add(timeFramedetails);

            return timeframe;
        }
        private TimeFramedetails GetFakeTimeFrameDetails(string _name, string _startTime, int _durationInMinute, bool _excludeHoliday) => new TimeFramedetails()
        {
            name = _name,
            startTime = _startTime,
            durationInMinutes = _durationInMinute,
            days = new List<string> { "MO", "TU", "WE", "TH", "FR", "SA", "SU" },
            excludeHolidays = _excludeHoliday
        };
        private DeviceNotifications GetFakeDeviceNotification() => new DeviceNotifications()
        {
            DeviceId = "",
            DeviceType = "",
            IsEventsSubscribed = false,
            IsMaintainanceSubscribed = false
        };
        private NotificationGroup GetFakeNotificationgroups() => new NotificationGroup()
        {
            AccountBU = "",
            ProductCluster = "",
            EventsGroupName = "",
            MaintainanceGroupName = "",
            DeviceNotifications = new List<DeviceNotifications>() { GetFakeDeviceNotification() }
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
            Devices = new List<DeviceResponse> { GetFakeDeviceResp() }
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
        private Timeframe GetFakeTimeFrames() => new Timeframe
        {
            count = 1,
            data = new List<TimeFramedetails> { GetFakeTimeFrameDetails("24x7x365", "00:00", 1336, false) }
        };
        #endregion
    }


}
