using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReviewNotifier.Helpers;
using ReviewNotifier.Models;
using TinyJson;

namespace ReviewNotifier
{
    public class Config : IConfig
    {
        public Settings GetSettings()
        {
            var location = "JsonSettings/settings.json".FullFileLocation();
            var fileJson = File.ReadAllText(location);
            var settings = fileJson.FromJson<Settings>();

            return settings;
        }
    }
}
