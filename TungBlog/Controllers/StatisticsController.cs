using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TungBlog.Data;
using TungBlog.Models;

namespace TungBlog.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly AppDbContext _context;

        public StatisticsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Statistics/Index
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }

        // POST: /Statistics/GetArticlesByAuthor
        [HttpPost]
        public JsonResult GetArticlesByAuthor(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Articles
                .Include(a => a.Author)
                .AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.SubmitDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.SubmitDate <= toDate.Value);
            }

            var stats = query
                .GroupBy(a => a.Author.FullName)
                .Select(g => new
                {
                    AuthorName = g.Key,
                    ArticleCount = g.Count()
                })
                .OrderByDescending(x => x.ArticleCount)
                .ToList();

            return Json(stats);
        }

        // POST: /Statistics/GetArticlesByCategory
        [HttpPost]
        public JsonResult GetArticlesByCategory(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Articles
                .AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.SubmitDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.SubmitDate <= toDate.Value);
            }

            var stats = query
                .GroupBy(a => a.Category)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    ArticleCount = g.Count()
                })
                .OrderByDescending(x => x.ArticleCount)
                .ToList();

            return Json(stats);
        }

        // POST: /Statistics/GetArticlesByStatus
        [HttpPost]
        public JsonResult GetArticlesByStatus(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Articles.AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.SubmitDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.SubmitDate <= toDate.Value);
            }

            var stats = query
                .GroupBy(a => a.Status)
                .Select(g => new
                {
                    Status = g.Key == 0 ? "Chờ duyệt" : g.Key == 1 ? "Đã duyệt" : "Từ chối",
                    ArticleCount = g.Count()
                })
                .OrderBy(x => x.Status)
                .ToList();

            return Json(stats);
        }
    }
}
