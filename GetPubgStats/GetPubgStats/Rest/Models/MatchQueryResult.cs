using GetPubgStats.Rest.JsonConverters;
using Newtonsoft.Json;
using System;

namespace GetPubgStats.Rest.Models
{
    public class MatchQueryResult
    {
        [JsonProperty("data")]
        public MatchData Data { get; set; }

        [JsonProperty("included")]
        public Participant[] Participants { get; set; }

        [JsonProperty("links")]
        public Link Link { get; set; }
    }

    public class MatchData
    {
        [JsonProperty("type")]
        public string Data { get; set; }

        [JsonProperty("id")]
        public string MatchId { get; set; }

        [JsonProperty("attributes")]
        public MatchAttributes Attributes { get; set; }

        [JsonProperty("relationships")]
        public MatchRelationships RelationShips { get; set; }

        [JsonProperty("links")]
        public Link Links { get; set; }
    }

    public class MatchAttributes
    {
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("duration"), JsonConverter(typeof(SecondsTimeSpanConverter))]
        public TimeSpan Duration { get; set; }

        [JsonProperty("gameMode"), JsonConverter(typeof(GameModeConverter))]
        public MatchGameMode GameMode { get; set; }

        [JsonProperty("mapName")]
        public string MapName { get; set; }

        [JsonProperty("isCustomMatch")]
        public bool IsCustomMatch { get; set; }

        [JsonProperty("patchVersion")]
        public string PatchVersion { get; set; }

        [JsonProperty("seasonState")]
        public SeasonState SeasonState { get; set; }

        [JsonProperty("shardId")]
        public string ShardId { get; set; }

        [JsonProperty("stats")]
        public object Stats { get; set; } //TODO: find object definition

        [JsonProperty("tags")]
        public object Tags { get; set; } //TODO: find object definition

        [JsonProperty("titleId")]
        public string TitleId { get; set; }
    }

    public class MatchRelationships
    {
        [JsonProperty("rosters"), JsonConverter(typeof(EncapsulatingNodeConverter<TypeIdPair>), "data")]
        public TypeIdPair[] Rosters { get; set; }

        [JsonProperty("assets"), JsonConverter(typeof(EncapsulatingNodeConverter<TypeIdPair>), "data")]
        public TypeIdPair[] Assets { get; set; }

        [JsonProperty("rounds")]
        public object Rounds { get; set; } //TODO: find object definition

        [JsonProperty("spectators")]
        public object Spectators { get; set; } //TODO: find object definitions
    }

    public enum MatchGameMode
    {
        Duo,
        DuoFpp,
        Solo,
        SoloFpp,
        Squad,
        SquadFpp,
        NormalDuo,
        NormalDuoFpp,
        NormalSolo,
        NormalSoloFpp,
        NormalSquad,
        NormalSquadFpp
    }

    public enum SeasonState
    {
        Closed,
        Prepare,
        Progress
    }
}
