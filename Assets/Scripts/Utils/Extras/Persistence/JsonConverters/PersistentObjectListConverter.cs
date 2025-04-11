using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace Honeylab.Utils.Persistence.JsonConverters
{
    internal class PersistentObjectListConverter : JsonConverter<List<PersistentObject>>
    {
        public override bool CanRead => false;


        public override List<PersistentObject> ReadJson(JsonReader reader,
            Type objectType,
            List<PersistentObject> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) => throw new NotImplementedException();


        public override void WriteJson(JsonWriter writer, List<PersistentObject> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            for (int persistentObjectIndex = 0; persistentObjectIndex < value.Count; persistentObjectIndex++)
            {
                PersistentObject runtimeObject = value[persistentObjectIndex];
                var bindings = runtimeObject._bindings;

                bool shouldSerializePersistentObject = false;
                for (int i = 0; i < bindings.Count && !shouldSerializePersistentObject; i++)
                {
                    shouldSerializePersistentObject = !(bindings[i].C is IShouldSerialize shouldSerialize) ||
                        shouldSerialize.ShouldSerialize;
                }

                if (shouldSerializePersistentObject)
                {
                    JObject serializedObject = JObject.FromObject(runtimeObject, serializer);
                    serializedObject.WriteTo(writer);
                }
            }

            writer.WriteEndArray();
        }
    }
}
