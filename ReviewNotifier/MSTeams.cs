using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace ReviewNotifier
{
    class MsTeams : IObserver
    {
        private readonly string _webHookUrl;
        private readonly string _json;
        private readonly LastIdSaver _lastIdSaver;

        public MsTeams()
        {
            var configuration = Configuration.configInstance;
            _webHookUrl = configuration.GetSection("WebHookUrl").Value;
            _json = GetJsonTemplate();
            _lastIdSaver = new LastIdSaver();
        }

        public void Update(ReviewInfo message)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(_webHookUrl);
            httpWebRequest.ContentType = "application/json";
            
            httpWebRequest.Method = "POST";

            var createdBy = message.CreatedBy.Split(" <FENERGO");
            
            var filledJsonTemplate = _json.Replace("$CREATEDBY", createdBy[0]).Replace("$TITLE", message.Title).Replace("$WORKITEMURL", message.WorkItemUrl);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(filledJsonTemplate);
            }

            try
            {
                var httpResponse = httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    _lastIdSaver.SaveValueToFile(message.Id);
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private string GetJsonTemplate()
        {
            TextReader textReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "NotificationTemplate.json"));
            return textReader.ReadToEnd();
        }
    }
}