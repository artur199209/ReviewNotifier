using System;
using System.IO;
using System.Reflection;

namespace ReviewNotifier
{
    class LastIdSaver : ILastIdSaver
    {
        public long GetValueFromFile()
        {
            string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "LastCodeReviewId.json"));
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            return Convert.ToInt64(jsonObj["ID"]);
        }

        public void SaveValueToFile(long id)
        {
            string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "LastCodeReviewId.json"));
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            jsonObj["ID"] = id.ToString();
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            //string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //string path2 = Path.GetFullPath(Path.Combine(path, @"..\\..\..\"));
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "LastCodeReviewId.json"), output);
        }
    }
}