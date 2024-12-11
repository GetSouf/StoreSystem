using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор записи

        [Required]
        public int OrderId { get; set; } // Связь с заказом

        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!; // Навигационное свойство

        [Required]
        public int ProductId { get; set; } // Связь с товаром

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!; // Навигационное свойство

        [Required]
        public int Quantity { get; set; } // Количество товара

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; } // Цена за единицу товара на момент заказа
    }

}
