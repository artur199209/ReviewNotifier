using System;
using System.IO;
using System.Net;
using log4net;
using ReviewNotifier.Helpers;
using ReviewNotifier.Interfaces;
using ReviewNotifier.Models;

namespace ReviewNotifier
{
    public class TeamsNotifier : INotifier
    {
        private readonly string _webHookUrl;
        private readonly string _json;
        private readonly ILog _logger;

        public TeamsNotifier(string webHookUrl)
        {
            _webHookUrl = webHookUrl;
            _json = GetJsonTemplate();
            _logger = Log4NetConfig.GetLogger();
        }

        public void Send(CodeReview message)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(_webHookUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            var filledJsonTemplate = _json.Replace("$CREATEDBY", message.CreatedBy.Replace("\\","\\\\")).Replace("$TITLE", message.Title).Replace("$WORKITEMURL", message.WorkItemUrl).Replace("$NAME", message.CreatedBy.Replace("\\", "\\\\"));
            
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
                    _logger.Info("Http response: " + result);
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.Error(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                _logger.Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private string GetJsonTemplate()
        {
            TextReader textReader = new StreamReader("JsonSettings/NotificationTemplate.json".FullFileLocation());
            return textReader.ReadToEnd();
        }
    }
}