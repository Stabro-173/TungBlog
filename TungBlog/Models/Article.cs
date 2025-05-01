namespace TungBlog.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public DateTime SubmitDate { get; set; }
        public int Status { get; set; } // 0: Pending, 1: Approved, 2: Rejected
        public string Category { get; set; }

        public int UserAccountId { get; set; }
        public virtual UserAccount? Author { get; set; }

        public int? CategoryId { get; set; }
        public virtual Category? CategoryRef { get; set; }
    }
}
