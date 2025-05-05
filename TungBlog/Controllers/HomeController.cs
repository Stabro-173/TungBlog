using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TungBlog.Models;
using TungBlog.Data;
using Microsoft.EntityFrameworkCore;

namespace TungBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            
            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Login", "Account");
            }

            if (role == "Admin")
            {
                return View();
            }
            else if (role == "Author")
            {
                return RedirectToAction("AuthorHome");
            }

            return RedirectToAction("Login", "Account");
        }

        public IActionResult AuthorHome()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Author")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var approvedArticles = _context.Articles
                .Include(a => a.Author)
                .Where(a => a.Status == 1) // Only approved articles
                .OrderByDescending(a => a.SubmitDate)
                .ToList();

            return View(approvedArticles);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return Content("Bạn không có quyền truy cập trang này.");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
