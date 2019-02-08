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
                .AddJsonFile("JsonSettings/DevInfo.json")
                .AddJsonFile("JsonSettings/settings.json");
            _configuration = builder.Build();

            return _configuration;
        }

        public static IConfigurationRoot configInstance
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = CreateConfiguration();
                }

                return _configuration;
            }
        }

    }
}