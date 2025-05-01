namespace TungBlog.Models
{
    public class AdminStatistics
    {
        public int TotalArticles { get; set; }
        public int PendingArticles { get; set; }
        public int ApprovedArticles { get; set; }
        public int RejectedArticles { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalCategories { get; set; }
    }
} 