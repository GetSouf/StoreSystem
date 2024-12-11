using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using StoreSystem.Models;
namespace StoreSystem.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор категории

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; // Название категории

        public int? ParentCategoryId { get; set; } // Родительская категория (если есть)

        [ForeignKey("ParentCategoryId")]
        public Category? ParentCategory { get; set; } // Навигационное свойство

        public ICollection<Category> SubCategories { get; set; } = new List<Category>(); // Подкатегории
        public ICollection<Product> Products { get; set; } = new List<Product>(); // Товары в категории
    }
}
