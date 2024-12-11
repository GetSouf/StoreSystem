using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор поставщика

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; // Название компании поставщика

        [MaxLength(100)]
        public string? ContactName { get; set; } // Контактное лицо

        [MaxLength(15)]
        public string? Phone { get; set; } // Телефон

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; } // Электронная почта

        public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>(); // Связь с товарами
    }
}
