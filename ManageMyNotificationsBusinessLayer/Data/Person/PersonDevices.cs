using System.Collections.Generic;
using Newtonsoft.Json;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class PersonDevices
    {

        public int Count { get; set; }
        public int Total { get; set; }
        public Person Person { get; set; }
        [JsonProperty("data")]
        public List<DeviceResponse> Devices { get; set; }
        public List<NotificationGroup> NotificationGroups { get; set; }
        public Links links { get; set; }

    }
}