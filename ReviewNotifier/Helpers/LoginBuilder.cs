using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using ReviewNotifier.Config;

namespace ReviewNotifier.Helpers
{
    public class LoginBuilder : ILoginBuilder
    {
        private readonly IConfigurationRoot _configuration;

        public LoginBuilder()
        {
            _configuration = Configuration.ConfigInstance;
        }

        public StringBuilder GetCreateByQuery()
        {
            var developersSection = _configuration.GetSection("developers");
            var developersArray = developersSection.AsEnumerable().ToList();

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("(");

            for (int i = 1; i < developersArray.Count; i++)
            {
                stringBuilder.Append(developersArray[i].Value);

                if (developersArray.Count - 1 != i)
                {
                    stringBuilder.Append(",");
                }
            }

            stringBuilder.Append(")");

            return stringBuilder;
        }
    }
}