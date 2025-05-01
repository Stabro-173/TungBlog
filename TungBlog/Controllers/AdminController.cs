using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TungBlog.Data;
using TungBlog.Models;

namespace TungBlog.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Articles
        public async Task<IActionResult> Articles()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var articles = await _context.Articles
                .Include(a => a.Author)
                .OrderByDescending(a => a.SubmitDate)
                .ToListAsync();

            return View(articles);
        }

        // GET: Admin/PendingArticles
        public async Task<IActionResult> PendingArticles()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var pendingArticles = await _context.Articles
                .Include(a => a.Author)
                .Where(a => a.Status == 0) // 0 means Pending
                .OrderByDescending(a => a.SubmitDate)
                .ToListAsync();

            return View(pendingArticles);
        }

        // GET: Admin/Statistics
        public async Task<IActionResult> Statistics()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var stats = new AdminStatistics
            {
                TotalArticles = await _context.Articles.CountAsync(),
                PendingArticles = await _context.Articles.CountAsync(a => a.Status == 0),
                TotalAuthors = await _context.UserAccounts.CountAsync(u => u.Role == "Author"),
                TotalCategories = await _context.Articles.Select(a => a.Category).Distinct().CountAsync()
            };

            // Get author statistics
            ViewBag.AuthorStats = await _context.Articles
                .GroupBy(a => a.Author)
                .Select(g => new
                {
                    g.Key.FullName,
                    ArticleCount = g.Count()
                })
                .OrderByDescending(x => x.ArticleCount)
                .ToListAsync();

            // Get category statistics
            ViewBag.CategoryStats = await _context.Articles
                .GroupBy(a => a.Category)
                .Select(g => new
                {
                    Name = g.Key,
                    ArticleCount = g.Count()
                })
                .OrderByDescending(x => x.ArticleCount)
                .ToListAsync();

            return View(stats);
        }

        // POST: Admin/UpdateArticleStatus
        [HttpPost]
        public async Task<IActionResult> UpdateArticleStatus(int id, int status)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            article.Status = status;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
} 