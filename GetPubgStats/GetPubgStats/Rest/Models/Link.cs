using Newtonsoft.Json;

namespace GetPubgStats.Rest.Models
{
    public class Link
    {
        [JsonProperty("self")]
        public string UrlToSelf { get; set; }
        [JsonProperty("schema")]
        public string Schema { get; set; }
    }
}
