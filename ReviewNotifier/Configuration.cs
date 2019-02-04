using System.IO;
using Microsoft.Extensions.Configuration;

namespace ReviewNotifier
{
    class Configuration
    {
        private static IConfigurationRoot _configuration;

        private static IConfigurationRoot CreateConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("DevInfo.json")
                .AddJsonFile("settings.json");
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