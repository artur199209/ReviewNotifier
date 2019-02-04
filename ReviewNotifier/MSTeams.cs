using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace ReviewNotifier
{
    class MsTeams : IObserver
    {
        private readonly string _webHookUrl;

        private readonly string _json;

        public MsTeams(string json)
        {
            _json = json;
            var configuration = Configuration.configInstance;
            _webHookUrl = configuration.GetSection("WebHookUrl").Value;
        }

        public void Update(ReviewInfo message)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(_webHookUrl);
            httpWebRequest.ContentType = "application/json";
            
            httpWebRequest.Method = "POST";

            var createdBy = message.CreatedBy.Split(" <FENERGO");
            var filledJsonTemplate = _json.Replace("$CREATEDBY", createdBy[0]).Replace("$TITLE", message.Title);

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
                    //save id to config
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}