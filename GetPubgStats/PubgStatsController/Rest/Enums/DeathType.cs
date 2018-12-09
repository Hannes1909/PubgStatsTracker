using System.Runtime.Serialization;

namespace PubgStatsController.Rest.Enums
{
    public enum DeathType
    {
        [EnumMember(Value = "alive")]
        Alive,
        [EnumMember(Value = "byplayer")]
        ByPlayer,
        [EnumMember(Value = "suicide")]
        Suicide,
        [EnumMember(Value = "logout")]
        Logout
    }
}
