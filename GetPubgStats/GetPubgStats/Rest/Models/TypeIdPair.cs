using Newtonsoft.Json;

namespace GetPubgStats.Rest.Models
{
    public class TypeIdPair
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
