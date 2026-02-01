using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication2.Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult UserProfile(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null) return RedirectToAction("Login", "Account");

            // 1. Profili görüntülenecek kullanýcýyý bul
            var targetUser = _context.Users.FirstOrDefault(u => u.Id == id);
            if (targetUser == null) return RedirectToAction("UserDashboard");

            // 2. Bu kullanýcýnýn paylaþtýðý tüm postlarý getir
            var userPosts = _context.Posts
                .Where(p => p.UserId == id)
                .OrderByDescending(p => p.CreatedDate)
                .ToList();

            // 3. Yorumlarý da göstermek istersen (isteðe baðlý)
            ViewBag.Comments = _context.Comments.ToList();
            ViewBag.TargetUser = targetUser;

            return View(userPosts);
        }
        // --- KULLANICI ARAMA (YENÝ EKLENDÝ - 404 HATASINI ÇÖZER) ---
        [HttpGet]
        public IActionResult SearchUser(string searchTerm)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            if (string.IsNullOrEmpty(searchTerm))
                return RedirectToAction("UserDashboard");

            // Küçük/büyük harf duyarsýz arama (Username veya Þehir içinde)
            var results = _context.Users
                .Where(u => (u.Username.ToLower().Contains(searchTerm.ToLower()) ||
                             (u.City != null && u.City.ToLower().Contains(searchTerm.ToLower())))
                             && u.Id != userId)
                .ToList();

            ViewBag.SearchTerm = searchTerm;
            return View(results);
        }

        // --- YORUM EKLEME ÝÞLEMÝ ---
        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, string commentContent)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var username = HttpContext.Session.GetString("Username");

            if (userId != null && !string.IsNullOrEmpty(commentContent))
            {
                var newComment = new Comment
                {
                    PostId = postId,
                    Content = commentContent,
                    UserId = userId.Value,
                    Username = username ?? "Anonim",
                    CreatedDate = DateTime.Now
                };

                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("UserDashboard");
        }

        // --- ANA SAYFA AKIÞI (DASHBOARD) ---
        public IActionResult UserDashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            ViewBag.User = user;
            ViewBag.Username = user?.Username;
            ViewBag.Role = user?.Role;

            // Beðeni listesi
            var userLikedPostIds = _context.Likes
                .Where(l => l.UserId == userId)
                .Select(l => l.PostId)
                .ToList();
            ViewBag.UserLikedPostIds = userLikedPostIds;

            // Tüm yorumlarý çek
            ViewBag.Comments = _context.Comments.OrderBy(c => c.CreatedDate).ToList();

            var posts = _context.Posts.OrderByDescending(p => p.CreatedDate).ToList();
            return View(posts);
        }

        // --- BEÐENÝ ÝÞLEMÝ ---
        [HttpPost]
        public async Task<IActionResult> LikePost(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var post = _context.Posts.Find(id);
            if (post == null) return RedirectToAction("UserDashboard");

            var existingLike = _context.Likes.FirstOrDefault(l => l.PostId == id && l.UserId == userId);

            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
                post.LikeCount = Math.Max(0, post.LikeCount - 1);
            }
            else
            {
                var newLike = new Like { PostId = id, UserId = userId.Value };
                _context.Likes.Add(newLike);
                post.LikeCount++;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("UserDashboard");
        }

        // --- POST SÝLME ---
        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var post = _context.Posts.Find(id);

            if (post != null && post.UserId == userId)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("UserDashboard");
        }

        public IActionResult Index()
        {
            var sessionUser = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(sessionUser))
                return RedirectToAction("Login", "Account");

            return RedirectToAction("UserDashboard");
        }

        [HttpPost]
        public async Task<IActionResult> SharePost(string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var username = HttpContext.Session.GetString("Username");

            if (userId != null && !string.IsNullOrEmpty(content))
            {
                var newPost = new Post
                {
                    Content = content,
                    UserId = userId.Value,
                    Username = username ?? "Anonim",
                    CreatedDate = DateTime.Now
                };

                _context.Posts.Add(newPost);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("UserDashboard");
        }

        [HttpGet]
        public IActionResult ProfileSettings()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User updatedData)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.City = updatedData.City;
                user.Age = updatedData.Age;
                user.Gender = updatedData.Gender;
                user.About = updatedData.About;
                user.Email = updatedData.Email;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("UserDashboard");
        }
    }
}