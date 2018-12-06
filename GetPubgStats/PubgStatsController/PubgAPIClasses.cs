using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Linq;
using PubgStatsController.Rest.Enums;

namespace PubgStatsController.Rest.Models
{
    /// <summary>
    /// returnobject for PubgAPI-Calls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Json<T>
    {
        private T cachedObject;

        public Json(string rawJson)
        {
            this.Value = rawJson;
        }

        public T AsObject() {
            if (this.cachedObject == null)
            {
                this.cachedObject = JsonConvert.DeserializeObject<T>(this.Value);
            } 
            return this.cachedObject; 
        }

        public string Value { get; private set; }
    } 

    /// <summary>
    /// class for own string-classes to typesafe calls
    /// </summary>
    public abstract class KeyString
    {
        public KeyString() { }

        public KeyString(string key)
        {
            this.Key = key;
        }

        public virtual string Key { get; set; }
        
        public static bool operator !=(KeyString obj1, KeyString obj2)
        {
            return obj1.Key != obj2.Key;
        }
        public static bool operator ==(KeyString obj1, KeyString obj2)
        {
            return obj1.Key == obj2.Key;
        }

        public override bool Equals(object obj)
        {
            return this.Key == ((KeyString)obj).Key;
        }
        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }
        public override string ToString()
        {
            return this.Key;
        }
    }


    [Obsolete("Currently unused")]
    /// <summary>
    /// extend IEnumerable for helper functions
    /// </summary>
    public static class IEnumerable_ValueString
    {
        public static string[] AsStringArray(this IEnumerable<KeyString> enum_valuestring)
        {
            return enum_valuestring.Select(_a => _a.Key).ToArray();
        }
        public static IEnumerable<string> AsStringEnumerable(this IEnumerable<KeyString> enum_valuestring)
        {
            return enum_valuestring.Select(_a => _a.Key);
        }
    }


    /// <summary>
    /// represent PUBG-Matchid
    /// </summary>
    public class SelectorMatchId : KeyString
    {
        public SelectorMatchId(string key) : base(key) {}
        public SelectorMatchId() { }

        public static implicit operator SelectorMatchId(string key)
        {
            return new SelectorMatchId(key);
        }

        public static implicit operator string(SelectorMatchId matchId)
        {
            return matchId.Key;
        }
    }

    /// <summary>
    /// represent PUBG-Accountid
    /// </summary>
    public class SelectorAccountId : KeyString
    {
        public SelectorAccountId(string key) : base(key) {}
        public SelectorAccountId() { }

        public static implicit operator SelectorAccountId(string key)
        {
            return new SelectorAccountId(key);
        }
        public static implicit operator string(SelectorAccountId accountId)
        {
            return accountId.Key;
        }
    }

    ///-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// JSON result as Object Playersearch 
    /// </summary>
    public class PlayerSearchResult
    {
        [JsonProperty("data")]
        public List<Player> Data { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }
    }

    /// <summary>
    /// Player Class
    /// </summary>
    public class Player
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        [JsonConverter(typeof(JsonConverterSelectorValue<SelectorAccountId>))]
        public SelectorAccountId AccountId { get; set; }

        [JsonProperty("attributes")]
        public PlayerAttributes Attributes { get; set; }

        public class PlayerAttributes
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("stats")]
            public object Stats { get; set; }

            [JsonProperty("titleId")]
            public string TitleId { get; set; }

            [JsonProperty("shardId")]
            public string ShardId { get; set; }

            [Obsolete]
            [JsonProperty("createdAt")]
            public DateTime CreatedAt { get; set; }

            [Obsolete]
            [JsonProperty("updatedAt")]
            public DateTime UpdatedAt { get; set; }

            [JsonProperty("patchVersion")]
            public string PatchVersion { get; set; }
        }

        [JsonProperty("relationships")]
        public PlayerRelationships Relationships { get; set; }
        public class PlayerRelationships
        {

            [JsonProperty("assets")]
            public PlayerAssets Assets { get; set; }
            public class PlayerAssets
            {
                [JsonProperty("data")]
                public List<object> Data { get; set; }
            }

            [JsonProperty("matches")]
            public PlayerMatches Matches { get; set; }
            public class PlayerMatches
            {
                [JsonProperty("data")]
                public List<Match> Data { get; set; }

                public class Match
                {
                    [JsonProperty("type")]
                    public string Type { get; set; }

                    [JsonProperty("id")]
                    [JsonConverter(typeof(JsonConverterSelectorValue<SelectorMatchId>))]
                    public SelectorMatchId MatchId { get; set; }
                }
            }
        }

        [JsonProperty("links")]
        public Links Links { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    /// <summary>
    /// JSON result as Object GetMatchdata
    /// </summary>
    public class Match
    {
        [JsonProperty("data")]
        public Matchdata Data { get; set; }

        public class Matchdata
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("id")]
            public SelectorMatchId MatchId { get; set; }

            [JsonProperty("attributes")]
            public MatchAttributes Attributes { get; set; }
            public class MatchAttributes
            {
                [JsonProperty("mapName")]
                public MapName Map { get; set; }

                [JsonProperty("isCustomMatch")]
                public bool IsCustomMatch { get; set; }

                [JsonProperty("duration")] //TODO: convert to timespan
                public int Duration { get; set; }

                [JsonProperty("gameMode")]
                public GameMode GameMode { get; set; }
                
                [JsonProperty("shardId")]
                public string ShardId { get; set; }

                [JsonProperty("tags")]
                public object Tags { get; set; }

                [JsonProperty("seasonState")]
                public SeasonState? SeasonState { get; set; }

                [JsonProperty("createdAt")]
                public DateTime CreatedAt { get; set; }

                [JsonProperty("stats")]
                public object Stats { get; set; }

                [JsonProperty("titleId")]
                public string TitleId { get; set; }
            }

            [JsonProperty("relationships")]
            public MatchRelationships Relationships { get; set; }

            [JsonProperty("links")]
            public Links Links { get; set; }
        }

        [JsonProperty("included")]
        public List<PlayerData> Included { get; set; }

        [JsonProperty("links")]
        public Links Links { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class TypeIdPair
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class MatchRelationships
    {
        [JsonProperty("rosters")]
        public MatchRosters Rosters { get; set; }
        public class MatchRosters
        {
            [JsonProperty("data")]
            public List<TypeIdPair> Data { get; set; }
        }

        [JsonProperty("assets")]
        public MatchAssets Assets { get; set; }
        public class MatchAssets
        {
            [JsonProperty("data")]
            public List<TypeIdPair> Data { get; set; }
        }
    }

    [JsonConverter(typeof(JsonConverterPlayerdata))]
    public class PlayerData : TypeIdPair
    {}

    public class PlayerdataParticipant : PlayerData
    {
        [JsonProperty("attributes")]
        public PlayerdataParticipantAttributes Attributes { get; set; }

        public class PlayerdataParticipantAttributes
        {
            [JsonProperty("shardId")]
            public string ShardId { get; set; }

            [JsonProperty("actor")]
            public string Actor { get; set; }

            [JsonProperty("stats")]
            public PlayerdataParticipantAttributesStats Stats { get; set; }
            public class PlayerdataParticipantAttributesStats
            {
                [JsonProperty("DBNOs")]
                public int? DBNOs { get; set; }

                [JsonProperty("assists")]
                public int? Assists { get; set; }

                [JsonProperty("boosts")]
                public int? Boosts { get; set; }

                [JsonProperty("damageDealt")]
                public double? DamageDealt { get; set; }

                [JsonProperty("deathType")]
                public DeathType? DeathType { get; set; }

                [JsonProperty("headshotKills")]
                public int? HeadshotKills { get; set; }

                [JsonProperty("heals")]
                public int? Heals { get; set; }

                [JsonProperty("killPlace")]
                public int? KillPlace { get; set; }

                [Obsolete("Deprecated for PC")]
                [JsonProperty("killPoints")]
                public int? KillPoints { get; set; }

                [Obsolete("Deprecated for PC")]
                [JsonProperty("killPointsDelta")]
                public double? KillPointsDelta { get; set; }

                [JsonProperty("killStreaks")]
                public int? KillStreaks { get; set; }

                [JsonProperty("kills")]
                public int? Kills { get; set; }

                [JsonProperty("lastKillPoints")]
                public int? LastKillPoints { get; set; }

                [JsonProperty("lastWinPoints")]
                public int? LastWinPoints { get; set; }

                [JsonProperty("longestKill")]
                public double? LongestKill { get; set; }

                [JsonProperty("mostDamage")]
                public int? MostDamage { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("playerId")]
                public SelectorAccountId PlayerId { get; set; }

                [JsonProperty("revives")]
                public int? Revives { get; set; }

                [JsonProperty("rideDistance")]
                public double? RideDistance { get; set; }

                [JsonProperty("roadKills")]
                public int? RoadKills { get; set; }

                [JsonProperty("swimDistance")]
                public double? SwimDistance { get; set; }

                [JsonProperty("teamKills")]
                public int? TeamKills { get; set; }

                [JsonProperty("timeSurvived")] //TODO: convert to TimeSpan
                public double? TimeSurvived { get; set; }

                [JsonProperty("vehicleDestroys")]
                public int? VehicleDestroys { get; set; }

                [JsonProperty("walkDistance")]
                public double? WalkDistance { get; set; }

                [JsonProperty("weaponsAcquired")]
                public int? PickedUpWeapons { get; set; }

                [JsonProperty("winPlace")]
                public int? GameRank { get; set; }

                [Obsolete("Deprecated for PC")]
                [JsonProperty("winPoints")]
                public int? WinPoints { get; set; }

                [Obsolete("Deprecated for PC")]
                [JsonProperty("winPointsDelta")]
                public double? WinPointsDelta { get; set; }
            }
        }
    }


    public class PlayerdataRoster : PlayerData
    {
        [JsonProperty("attributes")]
        public PlayerdataRoasterAttributes Attributes { get; set; }
        public class PlayerdataRoasterAttributes
        {
            [JsonProperty("shardId")]
            public string ShardId { get; set; }

            [JsonProperty("won")]
            public string WonMatch { get; set; }

            [JsonProperty("stats")]
            public PlayerdataRoasterAttributesStats Stats { get; set; }
            public class PlayerdataRoasterAttributesStats
            {
                [JsonProperty("rank")]
                public int Rank { get; set; }
                [JsonProperty("teamId")]
                public int TeamId { get; set; }
            }
        }

        [JsonProperty("relationships")]
        public PlayerdataRoasterRelationships Relationships { get; set; }
        public class PlayerdataRoasterRelationships
        {
            [JsonProperty("team")]
            public MatchTeam Team { get; set; }
            public class MatchTeam
            {
                [JsonProperty("data")]
                public object Data { get; set; }
            }

            [JsonProperty("participants")]
            public MatchParticipants Participants { get; set; }
            public class MatchParticipants
            {
                [JsonProperty("data")]
                public List<TypeIdPair> Data { get; set; }
            }
        }
    }


    public class PlayerdataAsset : PlayerData
    {
        [JsonProperty("attributes")]
        public PlayerdataAssetAttributes Attributes { get; set; }

        public class PlayerdataAssetAttributes
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("createdAt")]
            public DateTime? CreatedAt { get; set; }

            [JsonProperty("URL")]
            public string URL { get; set; }
        }
    }

    public class Meta
    {
    }


    public class Links
    {
        [JsonProperty("self")]
        public string Self { get; set; }

        [JsonProperty("schema")]
        public string Schema { get; set; }
    }


    class JsonConverterPlayerdata : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(PlayerData).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Newtonsoft.Json.Linq.JObject item = Newtonsoft.Json.Linq.JObject.Load(reader);

            switch (item.Value<string>("type"))
            {
                case "roster":
                    PlayerdataRoster _roster = new PlayerdataRoster();
                    serializer.Populate(item.CreateReader(), _roster);
                    return _roster;

                case "participant":
                    PlayerdataParticipant @_participant = new PlayerdataParticipant();
                    serializer.Populate(item.CreateReader(), @_participant);
                    return @_participant;

                case "asset":
                    PlayerdataAsset @_asset = new PlayerdataAsset();
                    serializer.Populate(item.CreateReader(), @_asset);
                    return @_asset;

                default:
                    return null;
            }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    class JsonConverterSelectorValue<T> : JsonConverter where T : KeyString, new()
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(PlayerData).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //Newtonsoft.Json.Linq.JObject item = Newtonsoft.Json.Linq.JObject.Load(reader);
            var item = reader.Value;

            T retvalue = new T();
            retvalue.Key = item.ToString();
            return retvalue;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }


}
