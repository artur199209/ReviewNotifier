using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ReviewNotifier.Helpers
{
    public static class LocationHelper
    {
        public static string FullFileLocation(this string filename)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), filename);
        }
    }
    public static class ConfigurationHelper
    {
        public static string GetString(this IConfigurationRoot config, string name)
        {
            return config.GetSection(name).Value;
        }
    }
}
