using System.Runtime.Serialization;

namespace PubgStatsController.PubgApi.Enums
{
    public enum MapName
    {
        [EnumMember(Value = "Desert_Main")]
        DesertMain,
        [EnumMember(Value = "Erangel_Main")]
        ErangelMain,
        [EnumMember(Value = "Savage_Main")]
        SavageMain,
        [EnumMember(Value = "Range_Main")]
        RangeMain
    }

}
