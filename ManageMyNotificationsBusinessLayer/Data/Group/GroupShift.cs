using Newtonsoft.Json;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class GroupShift
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("group")]
        public Group Group { get; set; }

        [JsonProperty("links")]
        public Links Link { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("start")]
        public string Start { get; set; }

        [JsonProperty("end")]
        public string End { get; set; }

        [JsonProperty("timezone")]
        public string TimeZone { get; set; }

        [JsonProperty("recurrence")]
        public Recurrence Recurrence { get; set; }

    }

    public class Recurrence
    {
        [JsonProperty("frequency")]
        public string Frequency { get; set; }

        [JsonProperty("repeatEvery")]
        public int RepeatEvery { get; set; }

        [JsonProperty("onDays")]
        public List<string> OnDays { get; set; }

        [JsonProperty("end")]
        public End End { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

    }

    public class End
    {
        [JsonProperty("endBy")]
        public string EndBy { get; set; }
    }
}
