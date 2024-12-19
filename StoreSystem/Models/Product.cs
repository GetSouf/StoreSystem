using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; 

        [Required]
        public int CategoryId { get; set; } 

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!; 

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; } 

        [Required]
        public int StockQuantity { get; set; } 

        [MaxLength(500)]
        public string? Description { get; set; }
        public double BonusPercentage { get; set; }


        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>(); 
        public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>(); 
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>(); 
}
