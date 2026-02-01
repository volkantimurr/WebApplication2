using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(user.Role))
                {
                    user.Role = "User";
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username ?? "Bilinmeyen");
                HttpContext.Session.SetString("Role", user.Role ?? "User");
                HttpContext.Session.SetInt32("UserId", user.Id);

                // YÖNLENDİRME MANTIĞI BURADA:
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // Admin dışındaki herkesi yeni yaptığımız sayfaya gönderiyoruz
                    return RedirectToAction("UserDashboard", "Home");
                }
            }

            ViewBag.Error = "Geçersiz kullanıcı adı veya şifre!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}