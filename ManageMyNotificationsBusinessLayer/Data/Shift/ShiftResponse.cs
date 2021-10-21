using Newtonsoft.Json;
using System;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class ShiftResponse
    {
        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("delay")]
        public int Delay { get; set; }

        [JsonProperty("escalationType")]
        public string EscalationType { get; set; }

        [JsonProperty("inRotation")]
        public Boolean InRotation { get; set; }

        [JsonProperty("shift")]
        public GroupShift GroupShift { get; set; }

        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
    }
    public class Recipient
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("targetName")]
        public string TargetName { get; set; }

        [JsonProperty("recipientType")]
        public string RecipientType { get; set; }

        [JsonProperty("deviceType")]
        public string DeviceType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }

    }
}
