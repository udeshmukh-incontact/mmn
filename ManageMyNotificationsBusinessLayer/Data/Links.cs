using Newtonsoft.Json;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class Links
    {
        [JsonProperty("self")]
        public string Self { get; set; }
        [JsonProperty("previous")]
        public string Previous { get; set; }
        [JsonProperty("next")]
        public string Next { get; set; }
    }
}
