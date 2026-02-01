using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using System.Linq;

namespace WebApplication2.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Kayıtlı tüm kullanıcıları listeler
        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        // Kullanıcı silme özelliği
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}