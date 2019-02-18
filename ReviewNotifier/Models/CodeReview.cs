

using System.Runtime.Serialization;

namespace ReviewNotifier.Models
{
    public class CodeReview
    {

        [DataMember(Name = "System.Id")]
        public int Id { get; set; }
        [DataMember(Name = "System.Title")]
        public string Title { get; set; }
        [DataMember(Name = "System.CreatedBy")]
        public string CreatedBy { get; set; }
        [DataMember(Name = "Microsoft.VSTS.CodeReview.Context")]
        public string Context { get; set; }
        public string WorkItemUrl { get; set; }

        public void BuildUrl(string tfsUrl)
        {
            WorkItemUrl = $"{tfsUrl}_versionControl/shelvesets?ss={Context};{CreatedBy}";
        }
    }
}