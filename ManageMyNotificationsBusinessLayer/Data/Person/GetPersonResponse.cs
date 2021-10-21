namespace ManageMyNotificationsBusinessLayer.Data
{
    public class GetPersonResponse
    {
        public string Id { get; set; }
        public string TargetName { get; set; }
        public string RecipientType { get; set; }
        public bool ExternallyOwned { get; set; }
        public Links Links { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Language { get; set; }
        public string Timezone { get; set; }
        public string WebLogin { get; set; }
        public Site Site { get; set; }
        public string Version { get; set; }
        public PersonRoles Roles { get; set; }
        public string Status { get; set; }
    }
}
