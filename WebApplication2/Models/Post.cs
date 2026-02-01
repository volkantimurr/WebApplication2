using System;

namespace WebApplication2.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } // Paylaşılan yazı
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Yazıyı paylaşan kullanıcının bilgisi
        public int UserId { get; set; }
        public string Username { get; set; }
        public int LikeCount { get; set; } = 0;
    }
}