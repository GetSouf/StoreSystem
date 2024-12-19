using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public int OrderId { get; set; } 

        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!; 

        [Required]
        public int ProductId { get; set; } 

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        [Required]
        public int Quantity { get; set; } 

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; } 
    }

}
