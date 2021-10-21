using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class DeviceRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string DeviceType { get; set; }
        public string EmailAddress { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public List<TimeFramedetails> Timeframes { get; set; }
    }
}
