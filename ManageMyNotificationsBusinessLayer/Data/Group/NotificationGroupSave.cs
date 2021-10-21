namespace ManageMyNotificationsBusinessLayer.Data
{
    public class NotificationGroupSave
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public bool IsSelectedPrevious { get; set; }
        public bool IsSelectedCurrent { get; set; }
        public bool IsAddDevice {
            get { return (this.IsSelectedCurrent == true && this.IsSelectedPrevious == false); }
        }
        public bool IsRemoveDevice
        {
            get { return (this.IsSelectedCurrent == false && this.IsSelectedPrevious == true); }
        }
    }
}
