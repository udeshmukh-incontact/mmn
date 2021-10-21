using Newtonsoft.Json;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class Timeframe
    {
        public int count { get; set; }
        [JsonProperty("data")]
        public List<TimeFramedetails> data { get; set; }

    }
    
    public class TimeFramedetails
    {
        public string name { get; set; }
        public string startTime { get; set; }
        public int durationInMinutes { get; set; }
        public List<string> days { get; set; }
        public bool excludeHolidays { get; set; }
    }
}
