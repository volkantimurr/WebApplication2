namespace WebApplication2.Models
{
    public class User
    {
        public int Id { get; set; }

        // Kullanıcı adı
        public string? Username { get; set; }

        // Şifre
        public string? Password { get; set; }

        // Rol (Varsayılan olarak "User" atandı)
        public string Role { get; set; } = "User"; // Buradaki fazladan virgülü sildim!

        // E-posta
        public string? Email { get; set; }

        public int? Age { get; set; }
        public string? Gender { get; set; } // "Erkek" veya "Kadın"
        public string? About { get; set; } // Hakkında bölümü

        public string? City { get; set; }
    }
}