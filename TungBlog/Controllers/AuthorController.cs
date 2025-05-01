using Microsoft.AspNetCore.Mvc;
using TungBlog.Data;
using TungBlog.Models;

namespace TungBlog.Controllers
{
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Author
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("AccessDenied", "Home");

            var authors = _context.UserAccounts
                .Where(u => u.Role == "Author")
                .ToList();

            return View(authors);
        }

        // GET: /Author/All
        public IActionResult All()
        {
            var authors = _context.UserAccounts
                .Where(u => u.Role == "Author")
                .ToList();

            return View(authors);
        }

        // GET: /Author/Add
        public IActionResult Add()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("AccessDenied", "Home");

            return View();
        }

        // POST: /Author/Add
        [HttpPost]
        public IActionResult Add(UserAccount author)
        {
            if (ModelState.IsValid)
            {
                author.Role = "Author"; // Gán cứng Role là Author
                _context.UserAccounts.Add(author);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(author);
        }

        // GET: /Author/Edit/{id}
        public IActionResult Edit(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("AccessDenied", "Home");

            var author = _context.UserAccounts.FirstOrDefault(u => u.Id == id && u.Role == "Author");
            if (author == null) return NotFound();

            return View(author);
        }

        // POST: /Author/Edit
        [HttpPost]
        public IActionResult Edit(UserAccount updatedAuthor)
        {
            if (ModelState.IsValid)
            {
                var author = _context.UserAccounts.FirstOrDefault(u => u.Id == updatedAuthor.Id && u.Role == "Author");
                if (author == null) return NotFound();

                author.FullName = updatedAuthor.FullName;
                author.DateOfBirth = updatedAuthor.DateOfBirth;
                author.Email = updatedAuthor.Email;
                author.Username = updatedAuthor.Username;
                author.Password = updatedAuthor.Password;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(updatedAuthor);
        }

        // GET: /Author/Delete/{id}
        public IActionResult Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin") return RedirectToAction("AccessDenied", "Home");

            var author = _context.UserAccounts.FirstOrDefault(u => u.Id == id && u.Role == "Author");
            if (author == null) return NotFound();

            _context.UserAccounts.Remove(author);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
