using Microsoft.AspNetCore.Mvc;

namespace TungBlog.Controllers
{
    public class AuthorHomeController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Author")
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View();
        }
    }
}
