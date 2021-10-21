using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsMVC.Models;

namespace ManageMyNotificationsMVC.Controllers.Converters
{
    public static class TimeFrameConverter
    {
        public static List<TimeFrame> ToViewModel(this Timeframe timeframe)
        {
            if (timeframe == null)
                return null;

            var viewModel = timeframe.data.Select(x => x.ToViewModel()).ToList();

            return viewModel;
        }

        public static TimeFrame ToViewModel(this TimeFramedetails data)
        {
            if (data == null)
                return null;

            TimeSpan _startTime = TimeSpan.Parse(data.startTime);
            TimeSpan _endTime = _startTime.Add(TimeSpan.FromMinutes(data.durationInMinutes));


            var viewModel = new TimeFrame
            {
                Name = data.name,
                Days = data.days,
                DurationInMinutes = data.durationInMinutes,
                ExcludeHolidays = data.excludeHolidays,
                StartTime = data.startTime,
                EndTime = _endTime.Hours.ToString("00") + ":" + _endTime.Minutes.ToString("00")
            };
            return viewModel;
        }

        public static Timeframe ToDomainModel(this List<TimeFrame> viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new List<TimeFrame>();
                viewModel.Add(new TimeFrame()
                {
                    Name = "24x7x365",
                    StartTime = "12:00 AM",
                    DurationInMinutes = 1440,
                    Days = new List<string> { "MO", "TU", "WE", "TH", "FR", "SA", "SU" },
                    EndTime = "12:00 AM",
                    ExcludeHolidays = false
                });
            }

            string[] formats = { "hhmm", "hmm", @"hh\:mm", @"h\:mm\:ss", @"h:mm", @"h:mm tt" };
            var TimeFrameDetailList = new List<TimeFramedetails>();

            foreach (var model in viewModel)
            {
                int _duration = 1440;
                DateTime startTime, endTime;
                var a = DateTime.TryParseExact(model.StartTime, formats, CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out startTime);
                
                var b = DateTime.TryParseExact(model.EndTime, formats, CultureInfo.CurrentCulture,
                    DateTimeStyles.None, out endTime);

                TimeSpan _endTime = endTime.TimeOfDay;
                TimeSpan _startTime = startTime.TimeOfDay;

                if (model.Name != "24x7x365" && !_endTime.Equals(_startTime))
                {
                    if (model.EndTime == "12:00 AM")
                    {
                        _duration = Convert.ToInt32(_endTime.Subtract(_startTime).TotalMinutes) + 1440;
                    }
                    else
                    {
                        _duration = Convert.ToInt32(_endTime.Subtract(_startTime).TotalMinutes);
                    }
                }

                var viewmodeldata = new TimeFramedetails
                {
                    name = model.Name,
                    startTime = _startTime.Hours.ToString("00") + ":" + _startTime.Minutes.ToString("00"),
                    days = model.Days,
                    durationInMinutes = _duration,
                    excludeHolidays = model.ExcludeHolidays,
                };

                TimeFrameDetailList.Add(viewmodeldata);
            }

            var domain = new Timeframe
            {
                count = TimeFrameDetailList.Count(),
                data = TimeFrameDetailList
            };

            return domain;
        }

    }
}