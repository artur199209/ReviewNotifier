using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using TinyJson;

namespace ReviewNotifier.Helpers
{
    public static class LocationHelper
    {
        public static string FullFileLocation(this string filename)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), filename);
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
                return responseBody.FromJson<T>();
            }
        }
        public static T PostWithResponse<T>(this HttpClient client, string url, string data)
        {
            var json = new { query = data }.ToJson();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = client.PostAsync(url, content).Result)
            {
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;
                return responseBody.FromJson<T>();
            }
        }
    }
}
