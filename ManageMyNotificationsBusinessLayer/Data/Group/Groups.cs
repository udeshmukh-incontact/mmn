using Newtonsoft.Json;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class Groups
    {
        public int Count { get; set; }
        public int Total { get; set; }
        [JsonProperty("data")]
        public List<Group> GroupMember { get; set; }
        public Links links { get; set; }
    }
}
