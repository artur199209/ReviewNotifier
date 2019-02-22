﻿using System;
using System.IO;
using System.Net;
using System.Xml.Linq;
using ReviewNotifier.Helpers;
using ReviewNotifier.Interfaces;
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
            Logger.Write("Sending review");

            var httpWebRequest = (HttpWebRequest) WebRequest.Create(_webHookUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            var filledJsonTemplate = _json.Replace("$CREATEDBY", message.CreatedBy.Replace("\\","\\\\")).Replace("$TITLE", message.Title).Replace("$WORKITEMURL", message.WorkItemUrl);

            Logger.Write(filledJsonTemplate);

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
                    Logger.Write("Http Response " + result);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
        }

        private string GetJsonTemplate()
        {
            TextReader textReader = new StreamReader("JsonSettings/NotificationTemplate.json".FullFileLocation());
            return textReader.ReadToEnd();
        }
    }
}