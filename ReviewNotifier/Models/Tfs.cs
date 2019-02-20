namespace ReviewNotifier.Models
{
    public class WorkItemResults
    {
        public Workitem[] WorkItems { get; set; }
        public WorkItemDataWrapper[] Value { get; set; }
    }

    public class Workitem
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }

    public class WorkItemDataWrapper
    {
        public CodeReview Fields { get; set; }
    }
}
