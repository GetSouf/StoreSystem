using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Models
{
    public class ProductSupplier
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор

        [Required]
        public int ProductId { get; set; } // Связь с товаром

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!; // Навигационное свойство

        [Required]
        public int SupplierId { get; set; } // Связь с поставщиком

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; } = null!; // Навигационное свойство

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal SupplyPrice { get; set; } // Цена закупки
    }
}
