using Microsoft.EntityFrameworkCore;
// User sınıfı aynı klasördeyse aşağıdaki satıra gerek yok, 
// ama hata devam ederse bunu ekleyin:
using WebApplication2.Models;

namespace WebApplication2.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Hata burada 'User' kelimesindeydi, artık User.cs olduğu için düzelecek
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}