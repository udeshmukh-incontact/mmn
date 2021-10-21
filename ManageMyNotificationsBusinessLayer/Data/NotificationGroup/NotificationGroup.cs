using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class NotificationGroup
    {
        public string AccountBU { get; set; }
        public string PartnerPrefix { get; set; }
        public string ProductCluster { get; set; }

        public string EventsGroupName { get; set; }

        public string MaintainanceGroupName { get; set; }

        public List<string> ParentAccountIDs { get; set; }

        public List<DeviceNotifications> DeviceNotifications { get; set; }
    }

    public class DeviceNotifications
    {
        public string DeviceId { get; set; }

        public string DeviceType { get; set; }

        public bool IsEventsSubscribed { get; set; }

        public bool IsMaintainanceSubscribed { get; set; }
    }
}