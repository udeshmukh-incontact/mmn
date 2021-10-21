namespace ManageMyNotificationsBusinessLayer.Data
{
    public class Member
    {
        public string Id { get; set; }
        public string TargetName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RecipientType { get; set; }
        public Links links { get; set; }
    }
}
