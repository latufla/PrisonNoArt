using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace Honeylab.Utils.Persistence.JsonConverters
{
    internal class ComponentBindingListJsonConverter : JsonConverter<List<ComponentBinding>>
    {
        public override List<ComponentBinding> ReadJson(JsonReader reader,
            Type objectType,
            List<ComponentBinding> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var bindings = new List<ComponentBinding>();
            serializer.Populate(reader, bindings);

            for (int i = bindings.Count - 1; i >= 0; i--)
            {
                if (bindings[i] == null)
                {
                    bindings.RemoveAt(i);
                }
            }

            return bindings;
        }


        public override void WriteJson(JsonWriter writer,
            List<ComponentBinding> value,
            JsonSerializer serializer)
        {
            writer.WriteStartArray();

            for (int i = 0; i < value.Count; i++)
            {
                ComponentBinding binding = value[i];
                if (binding.C is IShouldSerialize shouldSerialize &&
                    !shouldSerialize.ShouldSerialize)
                {
                    continue;
                }

                JObject componentObject = JObject.FromObject(binding, serializer);
                componentObject.WriteTo(writer);
            }

            writer.WriteEndArray();
        }
    }
}
