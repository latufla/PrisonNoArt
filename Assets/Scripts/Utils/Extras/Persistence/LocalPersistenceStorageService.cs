using Honeylab.Utils.Persistence.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;


namespace Honeylab.Utils.Persistence
{
    public class LocalPersistenceStorageService : IPersistenceStorageService
    {
        private readonly string _filePath;
        private readonly string _extension;
        private readonly JsonSerializer _serializer;
        private List<PersistentObject> _objects;
        private bool _isLoaded;
        private bool _isReset;
        private bool _isPause;

        private string _postfix = "";
        private string _ending = "";


        public LocalPersistenceStorageService(string filePath,
            string extension,
            string componentsNamespace,
            IEnumerable<Assembly> componentsLookupAssemblies)
        {
            _filePath = filePath;
            _extension = extension;
            _serializer = CreateSerializer(componentsNamespace, componentsLookupAssemblies);
        }


        public void Init()
        {
            string path = GetFullFilePath();
            if (!File.Exists(path))
            {
                _objects = new List<PersistentObject>();
                Save();
            }

            Read();

            _isLoaded = true;
        }


        public void Clear()
        {
            _objects?.Clear();
            _isLoaded = false;
        }


        public void Save()
        {
            if (_isReset || _isPause)
            {
                return;
            }

            string path = GetFullFilePath();
            GzipCompress(path);
        }


        private void Read()
        {
            string path = GetFullFilePath();
            string s = GzipDecompress(path);
            using StringReader stream = new StringReader(s);
            using JsonTextReader reader = new JsonTextReader(stream);

            _objects = _serializer.Deserialize<List<PersistentObject>>(reader) ?? new List<PersistentObject>();
        }


        public void SaveJson(string path)
        {
            using StreamWriter stream = new StreamWriter(path);
            using JsonWriter writer = new JsonTextWriter(stream);

            _serializer.Serialize(writer, _objects);
        }


        private void GzipCompress(string path)
        {
            var bytes = Encoding.UTF8.GetBytes(GetSave());
            using FileStream compressedFile = File.Create(path);
            using (var gzipStream = new GZipStream(compressedFile, CompressionMode.Compress))
            {
                gzipStream.Write(bytes);
            }
        }


        private string GzipDecompress(string path)
        {
            using FileStream compressedFile = File.Open(path, FileMode.Open);
            using (var gzipStream = new GZipStream(compressedFile, CompressionMode.Decompress))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    gzipStream.CopyTo(memoryStream);
                    string result = Encoding.UTF8.GetString(memoryStream.ToArray());
                    return result;
                }
            }
        }


        public string GetSave()
        {
            using StringWriter str = new StringWriter();
            using JsonWriter writer = new JsonTextWriter(str);
            _serializer.Serialize(writer, _objects);
            return str.ToString();
        }


        public void Reset()
        {
            _isReset = true;

            string path = GetFullFilePath();
            File.Delete(path);
        }


        public void SetPause(bool isPause) => _isPause = isPause;


        public void Add(PersistentObject obj)
        {
            ThrowIfNotLoaded();
            _objects.Add(obj);
        }


        public void Remove(PersistenceId id)
        {
            int index = _objects.FindIndex(it => it._id.Equals(id));
            _objects.RemoveAt(index);
        }


        public IReadOnlyList<PersistentObject> GetAll()
        {
            ThrowIfNotLoaded();
            return _objects;
        }


        public void SetFileNamePostfix(string postfix)
        {
            _postfix = postfix;
        }


        public void SetFileNameEnding(string ending)
        {
            _ending = ending;
        }


        private void ThrowIfNotLoaded()
        {
            if (!_isLoaded)
            {
                throw new InvalidOperationException("Not Loaded");
            }
        }


        private static JsonSerializer CreateSerializer(string componentsNamespace,
            IEnumerable<Assembly> componentsLookupAssemblies) => JsonSerializer
            .Create(new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new PersistenceIdJsonConverter(),
                    new ComponentBindingJsonConverter(componentsNamespace, componentsLookupAssemblies),
                    new ComponentBindingListJsonConverter(),
                    new PersistentObjectListConverter(),
                    new Vector2IntJsonConverter()
                },
                Formatting = Newtonsoft.Json.Formatting.None
            });


        private string GetFullFilePath() => string.Concat(_filePath, _postfix, _ending, ".", _extension);
    }
}
