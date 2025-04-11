using Newtonsoft.Json;
using System;
using UnityEngine;


namespace Honeylab.Utils.Persistence.JsonConverters
{
    internal class Vector2IntJsonConverter : JsonConverter<Vector2Int>
    {
        public override bool CanRead => false;


        public override Vector2Int ReadJson(JsonReader reader,
            Type objectType,
            Vector2Int existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) => throw new NotImplementedException();


        public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(value.x));
            writer.WriteValue(value.x);
            writer.WritePropertyName(nameof(value.y));
            writer.WriteValue(value.y);
            writer.WriteEndObject();
        }
    }
}
