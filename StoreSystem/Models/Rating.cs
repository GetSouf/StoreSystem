using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using testproject.Models;

namespace StoreSystem.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор отзыва

        [Required]
        public int CustomerId { get; set; } // Связь с клиентом

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!; // Навигационное свойство

        [Required]
        public int ProductId { get; set; } // Связь с товаром

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!; // Навигационное свойство

        [Required]
        [Range(1, 5)]
        public int RatingValue { get; set; } // Оценка (1-5)

        [MaxLength(500)]
        public string? Comment { get; set; } // Текст отзыва

        [Required]
        public DateTime RatingDate { get; set; } // Дата отзыва
    }
}
