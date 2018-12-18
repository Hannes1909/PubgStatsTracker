using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace PubgStatsController.Rest.JsonConverters
{
    public class EncapsulatingNodeConverter<T> : JsonConverter<T[]>
    {
        private readonly string encapsulatingNode;

        public EncapsulatingNodeConverter(object encapsulatingNode)
        {
            this.encapsulatingNode = encapsulatingNode?.ToString();
        }

        public EncapsulatingNodeConverter() : this("data")
        { }

        public override T[] ReadJson(JsonReader reader, Type objectType, T[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject value = JObject.Load(reader);
            return value[this.encapsulatingNode].ToObject<T[]>();
        }

        public override void WriteJson(JsonWriter writer, T[] value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Writing JSON is not yet supported!");
        }
    }
}
