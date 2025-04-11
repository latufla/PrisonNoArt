using Newtonsoft.Json;
using System;


namespace Honeylab.Utils.Persistence.JsonConverters
{
    internal class PersistenceIdJsonConverter : JsonConverter<PersistenceId>
    {
        public override PersistenceId ReadJson(JsonReader reader,
            Type objectType,
            PersistenceId existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            PersistenceId id = PersistenceId.CreateWithGuidString((string)reader.Value);
            return id;
        }


        public override void WriteJson(JsonWriter writer, PersistenceId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
