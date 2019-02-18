using System;
using System.IO;
using System.Net;
using ReviewNotifier.Helpers;
using ReviewNotifier.Models;

namespace ReviewNotifier
{
    public class TeamsNotifier : INotifier
    {
        private readonly string _webHookUrl;
        private readonly string _json;

        public TeamsNotifier(string webHookUrl)
        {
            _webHookUrl = webHookUrl;
            _json = GetJsonTemplate();
        }

        public void Send(CodeReview message)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(_webHookUrl);
            httpWebRequest.ContentType = "application/json";
            
            httpWebRequest.Method = "POST";

            var filledJsonTemplate = _json.Replace("$CREATEDBY", message.CreatedBy.Replace("\\","\\\\")).Replace("$TITLE", message.Title).Replace("$WORKITEMURL", message.WorkItemUrl);

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