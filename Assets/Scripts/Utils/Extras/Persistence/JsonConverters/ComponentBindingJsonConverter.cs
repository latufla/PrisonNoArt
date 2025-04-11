using Honeylab.Utils.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Honeylab.Utils.Persistence.JsonConverters
{
    internal class ComponentBindingJsonConverter : JsonConverter<ComponentBinding>
    {
        private readonly string _namespaceWithTypeSeparator;
        private readonly Assembly[] _lookupAssemblies;


        public ComponentBindingJsonConverter(string @namespace, IEnumerable<Assembly> lookupAssemblies)
        {
            _namespaceWithTypeSeparator = $"{@namespace}.";
            _lookupAssemblies = lookupAssemblies.ToArray();
        }


        public override ComponentBinding ReadJson(JsonReader reader,
            Type objectType,
            ComponentBinding existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            JObject rawObject = JObject.Load(reader);
            ComponentBinding binding = new();
            string name = (string)rawObject[nameof(binding.N)];
            string nameWithPersistentComponent = string.Concat(name, nameof(PersistentComponent));
            string fullTypeName = GetFullTypeName(nameWithPersistentComponent);
            Type componentType = _lookupAssemblies.Select(a => a.GetType(fullTypeName))
                .FirstOrDefault(t => t != null);
            if (componentType == null)
            {
                this.SelfLog($"Cannot find component type {fullTypeName}. Removing it");
                return null;
            }

            PersistentComponent component = (PersistentComponent)Activator.CreateInstance(componentType);
            JToken componentToken = rawObject[nameof(binding.C)];
            if (componentToken == null)
            {
                throw new InvalidOperationException("Component token not found");
            }

            using JsonReader componentTokenReader = componentToken.CreateReader();
            serializer.Populate(componentTokenReader, component);
            binding.C = component;
            return binding;
        }


        public override void WriteJson(JsonWriter writer, ComponentBinding value, JsonSerializer serializer)
        {
            string componentFullTypeName = value.C.GetType().FullName;
            if (string.IsNullOrEmpty(componentFullTypeName))
            {
                throw new InvalidOperationException();
            }

            string namePersistentComponent = componentFullTypeName.Replace(_namespaceWithTypeSeparator, string.Empty);
            string name = namePersistentComponent.Replace(nameof(PersistentComponent), string.Empty);
            JToken bindingObjectToken = new JObject
            {
                [nameof(value.N)] = JToken.FromObject(name, serializer),
                [nameof(value.C)] = JToken.FromObject(value.C, serializer)
            };

            bindingObjectToken.WriteTo(writer);
        }


        private string GetFullTypeName(string name) => $"{_namespaceWithTypeSeparator}{name}";
    }
}
