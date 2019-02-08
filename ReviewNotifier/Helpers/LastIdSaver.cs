using System;
using System.IO;
using System.Reflection;

namespace ReviewNotifier.Helpers
{
    class LastIdSaver : ILastIdSaver
    {
        private Object GetJsonContent()
        {
            try
            {
                string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "JsonSettings/LastCodeReviewId.json"));
                return Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public long GetValueFromFile()
        {
            try
            {
                dynamic jsonObj = GetJsonContent();
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
                dynamic jsonObj = GetJsonContent();
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