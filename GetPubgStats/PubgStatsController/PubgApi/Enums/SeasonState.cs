﻿using System.Runtime.Serialization;

namespace PubgStatsController.PubgApi.Enums
{
    public enum SeasonState
    {
        [EnumMember(Value = "closed")]
        Closed,
        [EnumMember(Value = "prepare")]
        Prepare,
        [EnumMember(Value = "progress")]
        Progress
    }
}
