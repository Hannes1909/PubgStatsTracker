using System.Collections.Generic;
using GetPubgStats.Rest.Models;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace GetPubgStats.Rest.JsonConverters
{
    public class GameModeConverter : JsonConverter<MatchGameMode>
    {
        public override MatchGameMode ReadJson(JsonReader reader, Type objectType, MatchGameMode existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string value = reader.Value?.ToString();
            return Enum.GetValues(typeof(MatchGameMode)).Cast<MatchGameMode>().Single(_mode => _mode.ToString().Equals(value.Replace("-", ""), StringComparison.InvariantCultureIgnoreCase));
        }

        public override void WriteJson(JsonWriter writer, MatchGameMode value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Serializing MatchGameMode is not supported yet!");
        }
    }

}
