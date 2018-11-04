using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Linq;

namespace PubgAPI
{
    /// <summary>
    /// returnobject for PubgAPI-Calls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Json<T>
    {
        string jsondata;
        T jsonobjectCached;

        public Json(string Jsondata)
        {
            this.jsondata = Jsondata;
        }

        public T AsObject() {
            if (this.jsonobjectCached == null)
            {
                this.jsonobjectCached = JsonConvert.DeserializeObject<T>(this.jsondata);
            } 
            return this.jsonobjectCached; 
        }

        public string Value { get { return this.jsondata; }}
    } 

    /// <summary>
    /// class for own string-classes to typesafe calls
    /// </summary>
    public abstract class KeyString
    {
        string value;

        public KeyString() { }
        public KeyString(string Key)
        {
            this.value = Key;
        }
        public string Key
        {
            get { return this.value; }
            set { this.value = value; }
        } 
        
        public static bool operator !=(KeyString obj1, KeyString obj2)
        {
            return obj1.value != obj2.value;
        }
        public static bool operator ==(KeyString obj1, KeyString obj2)
        {
            return obj1.value == obj2.value;
        }

        public override bool Equals(object obj)
        {
            return this.value == ((KeyString)obj).value;
        }
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
        public override string ToString()
        {
            return this.value;
        }
    }


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
    public class SelektorMatchid : KeyString
    {
        public SelektorMatchid(string Value) : base(Value) {}
        public SelektorMatchid() { }

        public static implicit operator SelektorMatchid(string value)
        {
            return new SelektorMatchid(value);
        }

        public static implicit operator string(SelektorMatchid value)
        {
            return value.Key;
        }
    }

    /// <summary>
    /// represent PUBG-Accountid
    /// </summary>
    public class SelektorAccountid : KeyString
    {
        public SelektorAccountid(string Value) : base(Value) {}
        public SelektorAccountid() { }

        public static implicit operator SelektorAccountid(string value)
        {
            return new SelektorAccountid(value);
        }
        public static implicit operator string(SelektorAccountid value)
        {
            return value.Key;
        }
    }

    ///-----------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// JSON result as Object Playersearch 
    /// </summary>
    public class PlayerSearchResult
    {
        public List<Player> data { get; set; }
        public Links links { get; set; }
    }

    /// <summary>
    /// Player Class
    /// </summary>
    public class Player
    {
        public string type { get; set; }
        [JsonConverter(typeof(JsonConverterSelectorValue<SelektorAccountid>))]
        public SelektorAccountid id { get; set; }
        public Attributes attributes { get; set; }
        public class Attributes
        {
            public string name { get; set; }
            public object stats { get; set; }
            public string titleId { get; set; }
            public string shardId { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
            public string patchVersion { get; set; }
        }


        public Relationships relationships { get; set; }
        public class Relationships
        {
            public Assets assets { get; set; }
            public class Assets
            {
                public List<object> data { get; set; }
            }

            public Matches matches { get; set; }
            public class Matches
            {
                public List<Match> data { get; set; }

                public class Match
                {
                    public string type { get; set; }
                    [JsonConverter(typeof(JsonConverterSelectorValue<SelektorMatchid>))]
                    public SelektorMatchid id { get; set; }
                }
            }
        }

        public Links links { get; set; }
        public Meta meta { get; set; }
    }

    /// <summary>
    /// JSON result as Object GetMatchdata
    /// </summary>
    public class Match
    {
        public Matchdata data { get; set; }
        public class Matchdata
        {
            public string type { get; set; }
            public SelektorMatchid id { get; set; }

            public MatchAttributes attributes { get; set; }
            public class MatchAttributes
            {
                public MatchdataMapName mapName { get; set; }
                public enum MatchdataMapName { Desert_Main, Erangel_Main, Savage_Main, Range_Main }
                public bool isCustomMatch { get; set; }
                public int duration { get; set; }
                public MatchdataGameMode gameMode { get; set; }
                public enum MatchdataGameMode { 
                    duo, 
                    [EnumMember( Value = "duo-fpp" )]
                    duo_fpp, 
                    solo, 
                    [EnumMember( Value = "solo-fpp" )]
                    solo_fpp, 
                    squad, 
                    [EnumMember( Value = "squad-fpp" )]
                    squad_fpp, 
                    [EnumMember( Value = "normal-duo" )]
                    normal_duo, 
                    [EnumMember( Value = "normal-duo-fpp" )]
                    normal_duo_fpp, 
                    [EnumMember( Value = "normal-solo" )]
                    normal_solo, 
                    [EnumMember( Value = "normal-solo-fpp" )]
                    normal_solo_fpp, 
                    [EnumMember( Value = "normal-squad" )]
                    normal_squad, 
                    [EnumMember( Value = "normal-squad-fpp" )]
                    normal_squad_fpp 
                }

                public string shardId { get; set; }
                public object tags { get; set; }
                public MatchdataSeasonState seasonState { get; set; }
                public enum MatchdataSeasonState { closed, prepare, progress }
                public DateTime createdAt { get; set; }
                public object stats { get; set; }
                public string titleId { get; set; }
            }

            public Relationships relationships { get; set; }
            public Links links { get; set; }
        }

        public List<PlayerData> included { get; set; }
        public Links links { get; set; }
        public Meta meta { get; set; }
    }



    public class Type_ID
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Relationships
    {
        public Rosters rosters { get; set; }
        public class Rosters
        {
            public List<Type_ID> data { get; set; }
        }

        public Assets assets { get; set; }
        public class Assets
        {
            public List<Type_ID> data { get; set; }
        }
    }



    [JsonConverter(typeof(JsonConverterPlayerdata))]
    public class PlayerData
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class PlayerdataParticipant : PlayerData
    {
        public PlayerdataParticipantAttributes attributes { get; set; }
        public class PlayerdataParticipantAttributes
        {
            public string shardId { get; set; }
            public string actor { get; set; }

            public PlayerdataParticipantAttributesStats stats { get; set; }
            public class PlayerdataParticipantAttributesStats
            {
                public int? DBNOs { get; set; }
                public int? assists { get; set; }
                public int? boosts { get; set; }
                public double? damageDealt { get; set; }
                public PlayerStatsDeathType? deathType { get; set; }
                public enum PlayerStatsDeathType { alive, byplayer, suicide, logout }
                public int? headshotKills { get; set; }
                public int? heals { get; set; }
                public int? killPlace { get; set; }
                public int? killPoints { get; set; }
                public double? killPointsDelta { get; set; }
                public int? killStreaks { get; set; }
                public int? kills { get; set; }
                public int? lastKillPoints { get; set; }
                public int? lastWinPoints { get; set; }
                public double? longestKill { get; set; }
                public int? mostDamage { get; set; }
                public string name { get; set; }
                public SelektorAccountid playerId { get; set; }
                public int? revives { get; set; }
                public double? rideDistance { get; set; }
                public int? roadKills { get; set; }
                public double? swimDistance { get; set; }
                public int? teamKills { get; set; }
                public double? timeSurvived { get; set; }
                public int? vehicleDestroys { get; set; }
                public double? walkDistance { get; set; }
                public int? weaponsAcquired { get; set; }
                public int? winPlace { get; set; }
                public int? winPoints { get; set; }
                public double? winPointsDelta { get; set; }
            }
        }
    }


    public class PlayerdataRoster : PlayerData
    {
        public PlayerdataRoasterAttributes attributes { get; set; }
        public class PlayerdataRoasterAttributes
        {
            public string shardId { get; set; }
            public string won { get; set; }

            public PlayerdataRoasterAttributesStats stats { get; set; }
            public class PlayerdataRoasterAttributesStats
            {
                public int rank { get; set; }
                public int teamId { get; set; }
            }
        }

        public PlayerdataRoasterRelationships relationships { get; set; }
        public class PlayerdataRoasterRelationships
        {
            public Team team { get; set; }
            public class Team
            {
                public object data { get; set; }
            }

            public Participants participants { get; set; }
            public class Participants
            {
                public List<Type_ID> data { get; set; }
            }
        }
    }


    public class PlayerdataAsset : PlayerData
    {
        public PlayerdataAssetAttributes attributes { get; set; }
        public class PlayerdataAssetAttributes
        {
            public string name { get; set; }
            public string description { get; set; }
            public DateTime? createdAt { get; set; }
            public string URL { get; set; }
        }
    }

    public class Meta
    {
    }


    public class Links
    {
        public string self { get; set; }
        public string schema { get; set; }
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
