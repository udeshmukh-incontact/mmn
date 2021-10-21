using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsMVC.Controllers.Converters;
using ManageMyNotificationsMVC.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ManageMyNotificationsMVCTests.Controllers.Converters
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class TimeFrameConverterTests
    {
        //[Test]
        //public void TimeFrameConverter_Test_ToViewModel()
        //{
        //    if (timeframe == null)
        //        return null;
        //    var data = timeframe.data.FirstOrDefault();

        //    TimeSpan _startTime = TimeSpan.Parse(data.startTime);
        //    TimeSpan _endTime = _startTime.Add(TimeSpan.FromMinutes(data.durationInMinutes));


        //    var viewModel = new TimeFrame
        //    {
        //        Name = data.name,
        //        Days = data.days,
        //        DurationInMinutes = data.durationInMinutes,
        //        ExcludeHolidays = data.excludeHolidays,
        //        StartTime = data.startTime,
        //        EndTime = _endTime.Hours.ToString("00") + ":" + _endTime.Minutes.ToString("00")
        //    };
        //    return viewModel;
        //    Assert.IsTrue(true);
        //}

        [Test]
        public void TimeFrameConverter_Test_Null_ToDomainModel()
        {
            List<TimeFrame> viewModel = null;

            var actual = viewModel.ToDomainModel();

            Assert.IsNotNull(actual);

        }

        [Test]
        public void TimeFrameConverter_Test_Null_ToViewModel()
        {
            TimeFramedetails viewModel = null;

            var actual = viewModel.ToViewModel();

            Assert.IsNull(actual);
        }


        [Test]
        public void TimeFrameConverter_Test_ToDomainModel_StartTimeEndTime_NotEqual()
        {
            List<TimeFrame> viewModel = new List<TimeFrame>()
            {
                new TimeFrame()
                {
                    StartTime = "01:00",
                    EndTime = "02:00",
                    Days = new List<string>() { "SU", "MO", "TU" },
                    Name = "24*7"
                }
            };
            var actual = viewModel.ToDomainModel();
            Assert.AreEqual(60, actual.data[0].durationInMinutes);
        }

        [Test]
        public void TimeFrameConverter_Test_ToDomainModel_EndTime_12_Hour_1380()
        {
            List<TimeFrame> viewModel = new List<TimeFrame>()
            {
                new TimeFrame()
                {
                    StartTime = "01:00 AM",
                    EndTime = "12:00 AM",
                    Days = new List<string>() { "SU", "MO", "TU" },
                    Name = "24*7"
                }
            };
            var actual = viewModel.ToDomainModel();
            Assert.AreEqual(1380, actual.data[0].durationInMinutes);
        }
    }
}
