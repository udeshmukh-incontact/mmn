using System;
using System.Collections.Generic;
using System.Linq;
using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsMVC.Models;

namespace ManageMyNotificationsMVC.Controllers.Converters
{
    public static class DeviceConverter
    {
        public static NotificationManagerViewModel ToViewModel(this PersonDevices domain)
        {
            if (domain == null)
                return null;
            var result = new NotificationManagerViewModel
            {
                Count = domain.Count,
                Total = domain.Total,

                Devices = domain.Devices.Select(p => new Device
                {
                    Id = p.Id,
                    Name = p.Name,
                    EmailAddress = p.EmailAddress,
                    DeviceType = p.DeviceType,
                    Description = p.Description,
                    PhoneNumber = p.PhoneNumber,
                    DeviceTimeFrame = p.TimeFrames.ToViewModel(),
                    CountryCode = p.Country
                }).ToList(),
            };
            return result;
        }
        public static PersonDevices ToDomainModel(this NotificationManagerViewModel viewModel)
        {
            if (viewModel == null)
            {
                return null;
            }
            var domain = new PersonDevices
            {
                Count = viewModel.Count,
                Total = viewModel.Total,

                Person = new Person()
                {
                    Id = string.IsNullOrEmpty(viewModel.Person.Id) ? (Guid?)null : Guid.Parse(viewModel.Person.Id),
                    Timezone = string.IsNullOrEmpty(viewModel.Person.TimeZone) ? null : viewModel.Person.TimeZone,
                    FirstName = viewModel.Person.FirstName,
                    TargetName = viewModel.Person.TargetName,
                    LastName = viewModel.Person.LastName,
                    Status = viewModel.Person.Status,
                    Language = viewModel.Person.Language
                },
                Devices = viewModel.Devices == null ? new List<DeviceResponse>() : viewModel.Devices.Select(p => new DeviceResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    EmailAddress = p.EmailAddress,
                    DeviceType = p.DeviceType,
                    Description = p.Description,
                    PhoneNumber = p.PhoneNumber,
                    TimeFrames = p.DeviceTimeFrame.ToDomainModel(),
                    Country = p.CountryCode
                }).ToList(),
                NotificationGroups = viewModel.NotificationGroups
            };
            return domain;
        }
        public static bool IsEquals(this DeviceResponse device1, DeviceResponse device2)
        {
            if (device1.DeviceType == "TEXT_PHONE" && device2.DeviceType == "TEXT_PHONE")
            {
                if (device1.Name == device2.Name &&
                    device1.Description == device2.Description &&
                    device1.PhoneNumber == device2.PhoneNumber &&
                    device1.Country == device2.Country &&
                    device1.TimeFrames.data.IsEquals(device2.TimeFrames.data))
                    return true;
                else
                    return false;
            }
            else if (device1.DeviceType == "EMAIL" && device2.DeviceType == "EMAIL")
            {
                if (device1.Name == device2.Name &&
                    device1.Description == device2.Description &&
                    device1.EmailAddress == device2.EmailAddress)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static bool IsEquals(this TimeFramedetails timeframe1, TimeFramedetails timeframe2)
        {
            if (timeframe1.name == timeframe2.name &&
               timeframe1.startTime == timeframe2.startTime &&
               timeframe1.days.SequenceEqual(timeframe2.days) &&
               timeframe1.durationInMinutes == timeframe2.durationInMinutes)
                return true;
            else
                return false;
        }
        public static bool IsEquals(this List<TimeFramedetails> timeframe1, List<TimeFramedetails> timeframe2)
        {
            if (timeframe1.Count != timeframe2.Count)
                return false;
            else
            {
                for (int i = 0; i < timeframe1.Count; i++)
                {
                    if (!timeframe1[i].IsEquals(timeframe2[i]))
                        return false;
                }
            }
            return true;
        }
    }
}
