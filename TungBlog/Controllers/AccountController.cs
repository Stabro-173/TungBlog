using Microsoft.AspNetCore.Mvc;
using TungBlog.Data;
using TungBlog.Models;
using TungBlog.Services;

namespace TungBlog.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Profile
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var user = _context.UserAccounts.Find(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: UpdateProfile
        [HttpPost]
        public IActionResult UpdateProfile(UserAccount updatedUser, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var user = _context.UserAccounts.Find(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("", "Mật khẩu mới và xác nhận mật khẩu không khớp");
                    return View("Profile", user);
                }
                user.Password = PasswordHasher.HashPassword(newPassword);
            }

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;

            if (ModelState.IsValid) //Kiểm tra xem dữ liệu có hợp lệ không (validation phía server).
            {
                _context.SaveChanges();
                TempData["Message"] = "Cập nhật thông tin thành công";
                return RedirectToAction("Profile");
            }

            return View("Profile", user);
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("Role") != null) // nếu khác null thì đã đăng nhập rồi
            {
                return RedirectToAction("Index", "Home"); //→ Không cho vào trang đăng ký nữa → chuyển về trang chủ.
            }
            return View();
        }

        // POST: Register
        [HttpPost]
        public IActionResult Register(UserAccount user)
        {
            ModelState.Remove("Role");
            user.Role = "Author";  //Gán mặc định vai trò người dùng là "Author"

            if (_context.UserAccounts.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                return View(user); 
            }

            if (_context.UserAccounts.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                user.Password = PasswordHasher.HashPassword(user.Password);
                _context.UserAccounts.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            HttpContext.Session.Clear(); // Clear any existing session
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.UserAccounts.FirstOrDefault(u => u.Username == username);

            if (user != null && PasswordHasher.VerifyPassword(password, user.Password))
            {
                // Set session variables
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("FullName", user.FullName);

                // Redirect based on role
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Articles", "Admin");
                }
                else if (user.Role == "Author")
                {
                    return RedirectToAction("Index", "AuthorHome");
                }
            }

            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
            return View();
        }

        // POST: Logout
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        // GET: Logout
        [HttpGet]
        public IActionResult LogoutGet()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
