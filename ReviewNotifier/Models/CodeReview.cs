using Newtonsoft.Json;

namespace ReviewNotifier.Models
{
    public class CodeReview
    {

        [JsonProperty(PropertyName = "System.Id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "System.Title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "System.CreatedBy")]
        public string CreatedBy { get; set; }
        [JsonProperty(PropertyName = "Microsoft.VSTS.CodeReview.Context")]
        public string Context { get; set; }
        public string WorkItemUrl { get; set; }

        public void BuildUrl(string tfsUrl)
        {
            WorkItemUrl = $"{tfsUrl}_versionControl/shelvesets?ss={Context};{CreatedBy}";
        }
    }
}