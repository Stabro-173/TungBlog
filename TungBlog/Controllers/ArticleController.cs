using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TungBlog.Data;
using TungBlog.Models;

namespace TungBlog.Controllers
{
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;

        public ArticleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Article/Index - List all articles
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var articles = _context.Articles
                .Include(a => a.Author)
                .OrderByDescending(a => a.SubmitDate)
                .ToList();

            return View(articles);
        }

        // GET: /Article/MyArticles - Author's articles
        public IActionResult MyArticles()
        {
            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (role != "Author" || !userId.HasValue)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var articles = _context.Articles
                .Where(a => a.UserAccountId == userId)
                .OrderByDescending(a => a.SubmitDate)
                .ToList();

            return View(articles);
        }

        // GET: /Article/PendingApproval - Review pending articles
        public IActionResult PendingApproval()
        {
            var role = HttpContext.Session.GetString("Role");
            
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var pendingArticles = _context.Articles
                .Include(a => a.Author)
                .Where(a => a.Status == 0)
                .OrderByDescending(a => a.SubmitDate)
                .ToList();

            return View(pendingArticles);
        }

        // POST: /Article/UpdateStatus
        [HttpPost]
        public IActionResult UpdateStatus(int id, int status)
        {
            var role = HttpContext.Session.GetString("Role");
            
            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var article = _context.Articles.Find(id);
            if (article == null) return NotFound();

            article.Status = status;
            _context.SaveChanges();

            return RedirectToAction("PendingApproval");
        }

        // GET: /Article/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            
            if (role != "Author")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }

        // POST: /Article/Create
        [HttpPost]
        public IActionResult Create(Article article)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Only validate required fields
                if (string.IsNullOrEmpty(article.Title) ||
                    string.IsNullOrEmpty(article.Category) ||
                    string.IsNullOrEmpty(article.Summary) ||
                    string.IsNullOrEmpty(article.Content))
                {
                    ModelState.AddModelError("", "Please fill in all article information");
                    return View(article);
                }

                article.SubmitDate = DateTime.Now;
                article.Status = 0; // Pending
                article.UserAccountId = userId.Value;
                article.Author = null; // Don't set navigation properties
                article.CategoryRef = null;

                _context.Articles.Add(article);
                _context.SaveChanges();

                TempData["Message"] = "Article has been submitted and is pending approval";
                return RedirectToAction("MyArticles");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Create: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the article. Please try again.");
                return View(article);
            }
        }

        // GET: /Article/Edit/{id}
        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            var article = _context.Articles.Find(id);
            if (article == null) return NotFound();

            if (role == "Author" && (article.UserAccountId != userId || article.Status != 0))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(article);
        }

        // POST: /Article/Edit
        [HttpPost]
        public IActionResult Edit(Article updatedArticle)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            var article = _context.Articles.Find(updatedArticle.Id);
            if (article == null) return NotFound();

            if (role == "Author" && (article.UserAccountId != userId || article.Status != 0))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ModelState.IsValid)
            {
                article.Title = updatedArticle.Title;
                article.Summary = updatedArticle.Summary;
                article.Content = updatedArticle.Content;
                article.Category = updatedArticle.Category;

                _context.SaveChanges();
                return RedirectToAction(role == "Admin" ? "Index" : "MyArticles");
            }

            return View(updatedArticle);
        }

        // GET: /Article/Details/{id}
        public IActionResult Details(int id)
        {
            var article = _context.Articles
                .Include(a => a.Author)
                .FirstOrDefault(a => a.Id == id);

            if (article == null) return NotFound();

            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (article.Status != 1 && role == "Author" && article.UserAccountId != userId)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(article);
        }

        // POST: /Article/Delete/{id}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            var article = _context.Articles.Find(id);
            if (article == null) return NotFound();

            if (role == "Author" && (article.UserAccountId != userId || article.Status != 0))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            _context.Articles.Remove(article);
            _context.SaveChanges();
            return RedirectToAction(role == "Admin" ? "Index" : "MyArticles");
        }
    }
}
