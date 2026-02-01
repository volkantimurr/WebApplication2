using System;

namespace WebApplication2.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // İlişkiler
        public int PostId { get; set; } // Hangi posta yapıldı?
        public int UserId { get; set; } // Kim yaptı?
        public string Username { get; set; } // Hızlıca göstermek için kullanıcı adı
    }
}
