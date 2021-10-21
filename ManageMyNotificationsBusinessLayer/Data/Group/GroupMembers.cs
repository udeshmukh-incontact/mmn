using Newtonsoft.Json;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class GroupMembers
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
        [JsonProperty("data")]
        public List<GroupMember> GroupMember { get; set; }
        [JsonProperty("links")]
        public Links links { get; set; }
    }
}
