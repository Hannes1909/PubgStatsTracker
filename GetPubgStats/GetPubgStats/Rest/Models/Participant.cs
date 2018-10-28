using GetPubgStats.Rest.JsonConverters;
using Newtonsoft.Json;
using System;

namespace GetPubgStats.Rest.Models
{
    public class Participant
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string ParticipantId { get; set; }

        [JsonProperty("attributes")]
        public ParticipantAttributes Attributes { get; set; }
    }

    public class ParticipantAttributes
    {
        [JsonProperty("stats")]
        public ParticipantStats Stats { get; set; }

        [JsonProperty("actor")]
        public string Actor { get; set; }

        [JsonProperty("shardId")]
        public string PlatformRegion { get; set; }
    }

    public class ParticipantStats
    {
        /// <summary>
        /// Number of enemy players knocked out
        /// </summary>
        [JsonProperty("DBNOs")]
        public int EnemyNockoutCount { get; set; }

        /// <summary>
        /// Number of kill assists
        /// </summary>
        [JsonProperty("assists")]
        public int AssistCount { get; set; }

        /// <summary>
        /// Number of boost items used
        /// </summary>
        [JsonProperty("boosts")]
        public int BoostItemUseCount { get; set; }

        /// <summary>
        /// Total amount of damage dealt. Self inflicted damage is substracted
        /// </summary>
        [JsonProperty("damageDealt")]
        public double DamageDealt { get; set; }

        /// <summary>
        /// Way the current player died, if the type is <see cref="PlayerDeathType.Alive"/>, 
        /// the player didn't die
        /// </summary>
        [JsonProperty("deathType")]
        public PlayerDeathType DeathType { get; set; }

        /// <summary>
        /// Number of enemies killed with headshot
        /// </summary>
        [JsonProperty("headshotKills")]
        public int HeadshotKillCount { get; set; }

        /// <summary>
        /// Number of used healing items
        /// </summary>
        [JsonProperty("heals")]
        public int HealItemUseCount { get; set; }

        /// <summary>
        /// Rank of the player based on killed enemies
        /// </summary>
        [JsonProperty("killPlace")]
        public int MatchRankKills { get; set; }

        [Obsolete("Deprecated for PC")]
        [JsonProperty("killPoints")]
        public int KillPoints { get; set; }

        [Obsolete("Deprecated for PC")]
        [JsonProperty("killPointsDelta")]
        public double KillPointsDelta { get; set; }

        /// <summary>
        /// Total number of kill streaks
        /// </summary>
        [JsonProperty("killStreaks")]
        public int KillStreakCount { get; set; }

        /// <summary>
        /// Number of killed enemies
        /// </summary>
        [JsonProperty("kills")]
        public int KillCount { get; set; }

        //TODO: find documentation
        [JsonProperty("lastKillPoints")]
        public int LastKillPoints { get; set; }

        //TODO: find documentation
        [JsonProperty("lastWinPoints")]
        public int LastWinPoints { get; set; }

        //TODO: find documentation
        [JsonProperty("longestKill")]
        public double LongestKill { get; set; }

        /// <summary>
        /// Amount of damage dealt with a single attack
        /// </summary>
        [JsonProperty("mostDamage")]
        public double MostDamageInOneAttack { get; set; }

        /// <summary>
        /// Ingame username of the current player
        /// </summary>
        [JsonProperty("name")]
        public string PlayerIngameName { get; set; }

        /// <summary>
        /// Id of the current player
        /// </summary>
        [JsonProperty("playerId")]
        public string PlayerId { get; set; }

        /// <summary>
        /// Number of revived teammates
        /// </summary>
        [JsonProperty("revives")]
        public int ReviveCount { get; set; }

        /// <summary>
        /// Total distance traveled in a vehicle in metres
        /// </summary>
        [JsonProperty("rideDistance")]
        public double VehicleRideDistance { get; set; }

        /// <summary>
        /// Number of kills by running over a player with a vehicle
        /// </summary>
        [JsonProperty("roadKills")]
        public int RoadKillCount { get; set; }

        /// <summary>
        /// Total distance swum metres
        /// </summary>
        [JsonProperty("swimDistance")]
        public double SwimDistance { get; set; }

        /// <summary>
        /// Number of teammate kills
        /// </summary>
        [JsonProperty("teamKills")]
        public int TeamKillCount { get; set; }

        /// <summary>
        /// Amount of time survived
        /// </summary>
        [JsonProperty("timeSurvived"), JsonConverter(typeof(SecondsTimeSpanConverter))]
        public TimeSpan TimeSurvived { get; set; }

        /// <summary>
        /// Number of vehicles destroyed
        /// </summary>
        [JsonProperty("vehicleDestroys")]
        public int VehicleDestoryCount { get; set; }

        /// <summary>
        /// Total distance walked in metres
        /// </summary>
        [JsonProperty("walkDistance")]
        public double WalkDistance { get; set; }

        /// <summary>
        /// Number of weapons picked up
        /// </summary>
        [JsonProperty("weaponsAcquired")]
        public int WeaponsAquiredCount { get; set; }

        [Obsolete("Deprecated for PC")]
        [JsonProperty("winPlace")]
        public int MatchRank { get; set; }

        [Obsolete("Deprecated for PC")]
        [JsonProperty("winPoints")]
        public int WinPoints { get; set; }

        [Obsolete("Deprecated for PC")]
        [JsonProperty("winPointsDelta")]
        public double WinPointsDelta { get; set; }
    }


    public enum PlayerDeathType
    {
        Alive,
        ByPlayer,
        Suicide,
        Logout
    }
}
