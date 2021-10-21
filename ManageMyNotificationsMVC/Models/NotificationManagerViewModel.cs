using ManageMyNotificationsBusinessLayer.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ManageMyNotificationsMVC.Models
{
    public class NotificationManagerViewModel
    {
        public int Count { get; set; }
        public int Total { get; set; }

        [JsonProperty()]
        public List<Device> Devices { get; set; }
        [JsonProperty()]
        public List<NotificationGroup> NotificationGroups { get; set; }
        [JsonProperty()]
        public Persons Person { get; set; }
    }
}