using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ReviewNotifier.Helpers;
using ReviewNotifier.Models;

namespace ReviewNotifier
{
    public class Config : IConfig
    {
        public Settings GetSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("JsonSettings/settings.json").Build();
            var settings = new Settings
            {
                WebHookUrl = configuration.GetString("webHookUrl").TrimEnd('/')+"/",
                TfsUrl = configuration.GetString("tfsUrl"),
                PersonalAccessTokenToTFS = configuration.GetString("personalAccessTokenToTFS"),
                Developers = configuration.GetSection("developers").AsEnumerable(true).Select(x => x.Value).ToList()
            };
            return settings;
        }
    }
}
