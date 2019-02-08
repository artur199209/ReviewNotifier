using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text;
using ReviewNotifier.Config;

namespace ReviewNotifier
{
    public class LoginBuilder : ILoginBuilder
    {
        private readonly IConfigurationRoot _configuration;

        public LoginBuilder()
        {
            _configuration = Configuration.configInstance;
        }

        public StringBuilder GetCreateByQuery()
        {
            var domain = _configuration.GetSection("Domain").Value;

            var developersSection = _configuration.GetSection("Developers");
            var developersArray = developersSection.AsEnumerable().ToList();

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("(");

            for (int i = 1; i < developersArray.Count; i++)
            {
                stringBuilder.Append(GetLogin(developersArray[i].Value, domain));

                if (developersArray.Count - 1 != i)
                {
                    stringBuilder.Append(",");
                }
            }

            stringBuilder.Append(")");

            return stringBuilder;
        }

        private string GetLogin(string name, string domain)
        {
            var person = name.Split(" ");
            var firstName = person[0];
            var lastName = person[1];
            var slash = "\\";
            var login = $"<{domain}{slash}{firstName[0]}{lastName}>";
            var loginPattern = $"'{firstName} {lastName} {login}'";

            return loginPattern;
        }
    }
}