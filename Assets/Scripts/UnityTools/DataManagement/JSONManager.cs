using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;

namespace UnityTools.DataManagement
{
    public static class JSONManager
    {

        private static string basePath = Application.streamingAssetsPath + "/JSON/";

        public static void Save<T>(string name, T item)
        {
            StreamWriter writer = new StreamWriter(basePath + name + ".json", false);
            writer.WriteLine(JsonConvert.SerializeObject(item, Formatting.Indented));
            writer.Close();
            Debug.Log(name + " saved");
        }

        public static T Load<T>(string name)
        {
            StreamReader reader = new StreamReader(basePath + name + ".json");
            string s = reader.ReadToEnd();
            reader.Close();
            return JsonConvert.DeserializeObject<T>(s);
        }

        public static Dictionary<string, T> LoadDirectory<T>(string directory)
        {
            Dictionary<string, T> result = new Dictionary<string, T>();

            foreach (string filePath in Directory.GetFiles(basePath + directory, "*.json"))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                result.Add(fileName, Load<T>(directory + "/" + fileName));
            }

            return result;
        }
    }
}