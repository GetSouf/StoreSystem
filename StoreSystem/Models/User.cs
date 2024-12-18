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
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Логин должен содержать только буквы и цифры.")]
        public string Username { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов.")]
        [MaxLength(100)]
        public string PasswordHash { get; set; } = null!; 

        [Required]
        public int PostId { get; set; }

        [ForeignKey("PostId")]
        public Post Post { get; set; } = null!; 

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; } = null!; 
    }
}
