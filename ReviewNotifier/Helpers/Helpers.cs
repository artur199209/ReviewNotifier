using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    public static class HttpHelper
    {

        public static T GetWithResponse<T>(this HttpClient client, string url)
        {
            using (HttpResponseMessage response = client.GetAsync(url).Result)
            {
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
        }
        public static T PostWithResponse<T>(this HttpClient client, string url, string data)
        {
            var json = JsonConvert.SerializeObject(new { query = data });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = client.PostAsync(url, content).Result)
            {
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
        }
    }
}
