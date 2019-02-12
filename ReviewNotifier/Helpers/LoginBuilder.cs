using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using ReviewNotifier.Config;

namespace ReviewNotifier.Helpers
{
    //TODO cleanup
    public class LoginBuilder : ILoginBuilder
    {
        private readonly IConfigurationRoot _configuration;

        public LoginBuilder()
        {
            _configuration = Configuration.ConfigInstance;
        }

        public StringBuilder GetCreateByQuery()
        {
            var domain = _configuration.GetSection("Domain").Value;

            var developersSection = _configuration.GetSection("developers");
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

        private static string GetLogin(string name, string domain)
        {
            const string slash = "\\";
            var person = name.Split(" ");
            var firstName = person[0];
            var lastName = person[1];
            var login = $"<FENERGO{slash}{firstName[0]}{lastName}>";
            var loginPattern = $"'{firstName} {lastName} {login}'";

            return loginPattern;
        }
    }
}