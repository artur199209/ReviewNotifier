using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ReviewNotifier.Models
{
    public class WorkItemData
    {
        [DataMember(Name = "System.Id")]
        public int Id { get; set; }
        [DataMember(Name = "System.Title")]
        public string Title { get; set; }
        [DataMember(Name = "System.CreatedBy")]
        public string CreatedBy { get; set; }
        [DataMember(Name = "Microsoft.VSTS.CodeReview.Context")]
        public string Context { get; set; }
        [DataMember(Name = "Microsoft.VSTS.CodeReview.ContextOwner")]
        public string ContextOwner { get; set; }
        public string Url { get; set; }
        public List<WorkItemData> WorkItems { get; set; } = new List<WorkItemData>();
        
        public void BuildShelvesetUrl(string url)
        {
            Url = $"{url}/_versionControl/shelveset?ss={Context};{ContextOwner}";
        }
        public void BuildWorkItemUrl(string url)
        {
            Url = $"{url}/_workitems/edit/{Id}";
        }

        public override string ToString()
        {
            return $"{Id} {Title} {CreatedBy}";
        }
    }
}