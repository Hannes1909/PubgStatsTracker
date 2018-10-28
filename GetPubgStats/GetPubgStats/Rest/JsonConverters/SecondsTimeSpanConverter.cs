using Newtonsoft.Json;
using System;

namespace GetPubgStats.Rest.JsonConverters
{
    public class SecondsTimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string value = reader.Value?.ToString() ?? "";
            if (Double.TryParse(value, out double duration))
            {
                var val = TimeSpan.FromSeconds(duration);
                return TimeSpan.FromSeconds(duration);
            }
            else
            {
                Int32.TryParse(value, out int durationSeconds);
                return new TimeSpan(0, 0, durationSeconds);
            }
        }

        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
