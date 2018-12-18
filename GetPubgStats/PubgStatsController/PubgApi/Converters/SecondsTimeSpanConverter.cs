using Newtonsoft.Json;
using System;

namespace PubgStatsController.PubgApi.Converters
{
    public class SecondsTimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string value = reader.Value?.ToString();

            if(Double.TryParse(value, out double exactSeconds))
            {
                return TimeSpan.FromSeconds(exactSeconds);
            }
            else if(Int32.TryParse(value, out int wholeSeconds))
            {
                return TimeSpan.FromSeconds(wholeSeconds);
            }

            throw new FormatException("Input value could not be parsed to a number");
        }

        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            writer.WriteValue((int)value.TotalSeconds);
        }
    }
}
