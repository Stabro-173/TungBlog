using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TungBlog.Data;
using TungBlog.Models;

namespace TungBlog.Controllers
{
    public class AuthorHomeController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorHomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var approvedArticles = _context.Articles
                .Include(a => a.Author)
                .Where(a => a.Status == 1) // Only approved articles
                .OrderByDescending(a => a.SubmitDate)
                .ToList();

            return View(approvedArticles);
        }
    }
}
