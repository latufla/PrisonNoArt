using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Honeylab.Utils.Configs.Editor
{
    public static class RemoteConfigCompiler
    {
        private static readonly string ConfigsInputPath = Application.streamingAssetsPath + "/../../Configs/";


        [MenuItem("Tools/Recompile Config")]
        public static void Compile()
        {
            string configsFilePath = RemoteConfigsService.ConfigsFilePath;
            string fileNameCompiled = Path.GetFileName(configsFilePath);

            var data = new Dictionary<string, object>
            {
                ["Timestamp"] = DateTime.UtcNow
            };

            foreach (string pathFile in Directory.EnumerateFiles(ConfigsInputPath,
                         "*.json",
                         SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(pathFile);

                if (fileName == fileNameCompiled)
                {
                    continue;
                }

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathFile);
                string fileContent = File.ReadAllText(pathFile);

                if (data.ContainsKey(fileNameWithoutExtension))
                {
                    throw new DuplicateConfigException(fileNameWithoutExtension);
                }

                try
                {
                    data.Add(fileNameWithoutExtension, JObject.Parse(fileContent));
                }
                catch (Exception)
                {
                    throw new InvalidConfigException(fileNameWithoutExtension);
                }
            }

            string json = JsonConvert.SerializeObject(data);
            if (configsFilePath != null)
            {
                File.WriteAllText(configsFilePath, json);
            }

            AssetDatabase.Refresh();
        }


        private class InvalidConfigException : Exception
        {
            public InvalidConfigException(string message) : base(message) { }
        }


        private class DuplicateConfigException : Exception
        {
            public DuplicateConfigException(string message) : base(message) { }
        }
    }
}
