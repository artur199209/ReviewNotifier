using System.IO;
using ReviewNotifier.Helpers;
using ReviewNotifier.Interfaces;
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
            settings.AdoUrl = EnsureTrailingSlash(settings.AdoUrl);
            settings.TfsUrl= EnsureTrailingSlash(settings.TfsUrl);


            return settings;
        }

        public string EnsureTrailingSlash(string url)
        {
            return url.EndsWith("/") ? url : url + "/";
        }
    }
}
