namespace ReviewNotifier.Models
{
    public class ReviewInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public string WorkItemUrl { get; set; }
    }
}