namespace ManageMyNotificationsBusinessLayer.Data
{
    public class DeviceResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string TargetName { get; set; }
        public string DeviceType { get; set; }
        public string Description { get; set; }
        public string TestStatus { get; set; }
        public bool ExternallyOwned { get; set; }
        public bool DefaultDevice { get; set; }
        public string PriorityThreshold { get; set; }
        public int Sequence { get; set; }
        public int Delay { get; set; }
        public Owner Owner { get; set; }
        public Links Links { get; set; }
        public string RecipientType { get; set; }
        public string Status { get; set; }
        public Provider Provider { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public Timeframe TimeFrames { get; set; }
    }
    
}
