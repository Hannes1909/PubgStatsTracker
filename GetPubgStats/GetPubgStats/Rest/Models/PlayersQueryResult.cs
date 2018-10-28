using GetPubgStats.Rest.JsonConverters;
using Newtonsoft.Json;
using System;

namespace GetPubgStats.Rest.Models
{
    public class PlayersQueryResult
    {
        [JsonProperty("data")]
        public Player[] Players { get; set; }

        [JsonProperty("links")]
        public Link Link { get; set; }
    }

    public class PlayerQueryResult
    {
        [JsonProperty("data")]
        public Player Player { get; set; }

        [JsonProperty("links")]
        public Link Link { get; set; }
    }

    public class Player
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("id")]
        public string AccountId { get; set; }

        [JsonProperty("attributes")]
        public PlayerAttributes Attributes { get; set; }

        [JsonProperty("relationships")]
        public PlayerRelationShips Relationships { get; set; }

        [JsonProperty("links")]
        public Link Link { get; set; }
    }

    public class PlayerAttributes
    {
        [Obsolete]
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Obsolete]
        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("patchVersion")]
        public string PatchVersion { get; set; }

        [JsonProperty("name")]
        public string Username { get; set; }

        [JsonProperty("stats")]
        public object Stats { get; set; } //TODO: find object definition

        [JsonProperty("titleId")]
        public string TitleId { get; set; }

        [JsonProperty("shardId")]
        public string PlatformRegion { get; set; }
    }

    public class PlayerRelationShips
    {
        [JsonProperty("assets")]
        public object Assets { get; set; } //TODO: find object definition

        [JsonProperty("matches"), JsonConverter(typeof(EncapsulatingNodeConverter<TypeIdPair>), "data")]
        public TypeIdPair[] Matches { get; set; }
    }
}
