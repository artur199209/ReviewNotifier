using System;
using System.IO;
using System.Linq;
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
        private readonly LastIdSettings _lastIdSettings;

        public TeamsNotifier(string webHookUrl)
        {
            _webHookUrl = webHookUrl;
            _json = GetJsonTemplate();
            _logger = Log4NetConfig.GetLogger();
            _lastIdSettings = new LastIdSettings();
        }

        public void Send(WorkItemData message)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(_webHookUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            var sas = message.WorkItems.Aggregate("", (current, messageWorkItem) => current + $"{{\"name\": \"{messageWorkItem.Id}:\",\"value\": \"[{messageWorkItem.Title}]({messageWorkItem.Url})\"}},");

            var filledJsonTemplate = _json.Replace("$CREATEDBY", message.CreatedBy.Replace("\\","\\\\"))
                .Replace("$TITLE", message.Title.Replace("\"", "\\\""))
                .Replace("$WORKITEMURL", message.Url)
                .Replace("$WorkItems", sas)
                .Replace("$NAME", message.CreatedBy.Replace("\\", "\\\\"));
            
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

                    if (result == "1")
                    {
                        _logger.Info("Saving ID");
                        _lastIdSettings.Save(message.Id);
                    }

                    _logger.Info("Http response: " + result);
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.Error(ex.InnerException);
                _logger.Error(ex.StackTrace);
            }
            catch (NotSupportedException ex)
            {
                _logger.Error(ex.InnerException);
                _logger.Error(ex.StackTrace);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.InnerException);
                _logger.Error(ex.StackTrace);
            }
        }

        private string GetJsonTemplate()
        {
            TextReader textReader = new StreamReader("JsonSettings/NotificationTemplate.json".FullFileLocation());
            return textReader.ReadToEnd();
        }
    }
}