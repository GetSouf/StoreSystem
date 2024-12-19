using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Models
{
    public class ProductSupplier
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public int ProductId { get; set; } 

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!; 

        [Required]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; } = null!; 

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal SupplyPrice { get; set; } 
    }
}
