using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using ReviewNotifier.Config;

namespace ReviewNotifier.Helpers
{
    public class LoginBuilder : ILoginBuilder
    {
        private readonly IConfiguration _configuration;

        public LoginBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetCreateByQuery()
        {
            var developersSection = _configuration.GetSection("developers");
            var developers = developersSection.AsEnumerable().Skip(1).Select(x => $"'{x.Value}'").ToList();
            var joinedDevs = string.Join(",", developers);
            var result = $"({joinedDevs})";
            return result;
        }
    }
}