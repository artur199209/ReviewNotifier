
using Newtonsoft.Json;

namespace ReviewNotifier.Models
{
    public class WorkItemResults
    {
        public Workitem[] workItems { get; set; }
        public WorkItemDataWrapper[] value { get; set; }
    }

    public class Workitem
    {
        public int id { get; set; }
        public string url { get; set; }
    }

    public class WorkItemDataWrapper
    {
        public CodeReview fields { get; set; }
    }
}
