using System.Collections.Generic;

namespace ReviewNotifier.Models
{
    public class Settings
    {
        public string WebHookUrl { get; set; }
        public string TfsUrl { get; set; }
        public string PersonalAccessTokenToTFS { get; set; }
        public string AdoUrl { get; set; }
        public string AdoToken { get; set; }
        public int CodeReviewCount { get; set; }
        public List<string> Developers { get; set; }
    }
}
