using System.Runtime.Serialization;

namespace PubgStatsController.Rest.Enums
{
    public enum GameMode
    {
        [EnumMember(Value = "solo")]
        Solo,
        [EnumMember(Value = "solo-fpp")]
        SoloFpp,
        [EnumMember(Value = "duo")]
        Duo,
        [EnumMember(Value = "duo-fpp")]
        DuoFpp,
        [EnumMember(Value = "squad")]
        Squad,
        [EnumMember(Value = "squad-fpp")]
        SquadFpp,
        [EnumMember(Value = "normal-duo")]
        NormalDuo,
        [EnumMember(Value = "normal-duo-fpp")]
        NormalDuoFpp,
        [EnumMember(Value = "normal-solo")]
        NormalSolo,
        [EnumMember(Value = "normal-solo-fpp")]
        NormalSoloFpp,
        [EnumMember(Value = "normal-squad")]
        NormalSquad,
        [EnumMember(Value = "normal-squad-fpp")]
        NormalSquadFpp
    }
}
