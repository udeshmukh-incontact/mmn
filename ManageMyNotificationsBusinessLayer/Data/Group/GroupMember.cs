using Newtonsoft.Json;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class GroupMember
    {
        [JsonProperty("Group")]
        public Group Group { get; set; }

        [JsonProperty("Member")]
        public Member Member { get; set; }
    }
}
