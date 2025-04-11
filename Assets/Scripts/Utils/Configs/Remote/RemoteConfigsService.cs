using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;
using UnityEngine;
#if !UNITY_EDITOR && UNITY_ANDROID
using UnityEngine.Networking;
#endif


namespace Honeylab.Utils.Configs
{
    public class RemoteConfigsService : IConfigsService
    {
        private JObject _jObject;


        public static string ConfigsFilePath => Path.Combine(Application.streamingAssetsPath, "Configs.json");
        public string ConfigsTimestamp { get; private set; } = "Not loaded yet";


        public async UniTask InitializeAsync(CancellationToken ct, IProgress<float> progress = null)
        {
            string filePath = ConfigsFilePath;
            string json = await ReadJsonFromFileAsync(filePath, ct, progress);

            _jObject = await UniTask.RunOnThreadPool(() => JObject.Parse(json), cancellationToken: ct);

            ConfigsTimestamp = GetConfigAsString("Timestamp");
        }


        public bool TryGet<T>(string configId, out T config)
        {
            if (!_jObject.TryGetValue(configId, out JToken value))
            {
                config = default;
                return false;
            }

            config = value.ToObject<T>();
            return true;
        }


        private string GetConfigAsString(string configId) => _jObject.TryGetValue(configId, out JToken value) ?
            value.ToString() :
            string.Empty;


        private static async UniTask<string> ReadJsonFromFileAsync(string filePath,
            CancellationToken ct,
            IProgress<float> progress = null)
        {
            #if !UNITY_EDITOR && UNITY_ANDROID
            UnityWebRequest webRequest = UnityWebRequest.Get(filePath);
            webRequest.timeout = 10;

            await webRequest.SendWebRequest().ToUniTask(progress, cancellationToken: ct);
            string json = webRequest.downloadHandler.text;
            #else
            string json = File.ReadAllText(filePath);
            progress?.Report(1.0f);
            #endif

            return json;
        }
    }
}
