using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string PasswordHash { get; set; } = null!; // Захэшированный пароль

        [Required]
        public int PostId { get; set; }

        [ForeignKey("PostId")]
        public Post Post { get; set; } = null!; // Навигационное свойство для роли
    }
}
