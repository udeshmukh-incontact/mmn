using Newtonsoft.Json;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class Group
    {        
        public string Id { get; set; }     
        public string TargetName { get; set; }
        public string RecipientType { get; set; }
        [JsonProperty("links")]
        public Links Links { get; set; }
    }
}
