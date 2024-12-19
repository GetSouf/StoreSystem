using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using testproject.Models;

namespace StoreSystem.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public int CustomerId { get; set; } 

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!; 

        [Required]
        public int ProductId { get; set; } /

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!; 

        [Required]
        [Range(1, 5)]
        public int RatingValue { get; set; } 

        [MaxLength(500)]
        public string? Comment { get; set; } 

        [Required]
        public DateTime RatingDate { get; set; } 
    }
}
