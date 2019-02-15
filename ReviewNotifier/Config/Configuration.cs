using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ReviewNotifier.Helpers;
using ReviewNotifier.Interfaces;
using ReviewNotifier.Models;

namespace ReviewNotifier.Config
{
    public class Configuration : IConfig
    {
        public Settings GetSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("JsonSettings/settings.json").Build();
            var settings = new Settings
            {
                WebHookUrl = configuration.GetString("webHookUrl"),
                TfsUrl = configuration.GetString("tfsUrl"),
                Project = configuration.GetString("project"),
                PersonalAccessTokenToTFS = configuration.GetString("personalAccessTokenToTFS"),
                Developers = configuration.GetSection("developers").AsEnumerable(true).Select(x => x.Value).ToList()
            };
            return settings;
        }
    }
}
