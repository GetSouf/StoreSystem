using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using StoreSystem.Models;
namespace StoreSystem.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; 

        public int? ParentCategoryId { get; set; } 

        [ForeignKey("ParentCategoryId")]
        public Category? ParentCategory { get; set; } 

        public ICollection<Category> SubCategories { get; set; } = new List<Category>(); 
        public ICollection<Product> Products { get; set; } = new List<Product>(); 
    }
}
