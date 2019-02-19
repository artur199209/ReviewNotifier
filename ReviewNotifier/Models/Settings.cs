using System;
using System.Collections.Generic;
using System.Text;

namespace ReviewNotifier.Models
{
    public class Settings
    {
        public string WebHookUrl { get; set; }
        public string TfsUrl { get; set; }
        public string PersonalAccessTokenToTFS { get; set; }
        public int CodeReviewCount { get; set; }
        public List<string> Developers { get; set; }
    }
}
