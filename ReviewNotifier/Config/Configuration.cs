using System.IO;
using Microsoft.Extensions.Configuration;

namespace ReviewNotifier.Config
{
    class Configuration
    {
        private static IConfigurationRoot _configuration;

        private static IConfigurationRoot CreateConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("JsonSettings/settings.json");
            _configuration = builder.Build();

            return _configuration;
        }

        public static IConfigurationRoot ConfigInstance => 
            _configuration ?? (_configuration = CreateConfiguration());
    }
}