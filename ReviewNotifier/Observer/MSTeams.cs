using System;
using System.IO;
using System.Net;
using System.Reflection;
using ReviewNotifier.Config;
using ReviewNotifier.Helpers;
using ReviewNotifier.Models;

namespace ReviewNotifier.Observer
{
    class MsTeams : IObserver
    {
        private readonly string _webHookUrl;
        private readonly string _json;

        public MsTeams()
        {
            var configuration = Configuration.ConfigInstance;
            _webHookUrl = configuration.GetSection("webHookUrl").Value;
            _json = GetJsonTemplate();
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
            TextReader textReader = new StreamReader("JsonSettings/NotificationTemplate.json".FullFileLocation());
            return textReader.ReadToEnd();
        }
    }
}