using System;
using System.IO;
using System.Reflection;

namespace ReviewNotifier
{
    class LastIdSaver : ILastIdSaver
    {
        public long GetValueFromFile()
        {
            try
            {
                string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "LastCodeReviewId.json"));
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                return Convert.ToInt64(jsonObj["ID"]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void SaveValueToFile(long id)
        {
            try
            {
                string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "LastCodeReviewId.json"));
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                jsonObj["ID"] = id.ToString();
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "LastCodeReviewId.json"), output);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}