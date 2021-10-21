using Newtonsoft.Json;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class GroupShifts
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("data")]
        public List<GroupShift> GroupShift { get; set; }

        [JsonProperty("links")]
        public Links Link { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
