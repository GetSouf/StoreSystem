using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор товара

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; // Название товара

        [Required]
        public int CategoryId { get; set; } // Связь с категорией

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!; // Навигационное свойство

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; } // Цена товара

        [Required]
        public int StockQuantity { get; set; } // Количество на складе

        [MaxLength(500)]
        public string? Description { get; set; } // Описание товара
        public double BonusPercentage { get; set; }


        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>(); // Связь с деталями заказов
        public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>(); // Связь с поставщиками
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>(); // Оценки и отзывы
    }
}
