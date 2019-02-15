using ReviewNotifier.Models;
using System.Linq;

namespace ReviewNotifier.Helpers
{
    public class LoginBuilder : ILoginBuilder
    {
        private readonly Settings _settings;

        public LoginBuilder(Settings settings)
        {
            _settings = settings;
        }

        public string GetCreateByQuery()
        {
            var developers = _settings.Developers.Select(x => $"'{x}'").ToList();
            var joinedDevs = string.Join(",", developers);
            var result = $"({joinedDevs})";
            return result;
        }
    }
}