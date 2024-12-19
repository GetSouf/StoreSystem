using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(100)]
        public string? ContactName { get; set; } 

        [MaxLength(15)]
        public string? Phone { get; set; } 

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; } 

        public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>(); 
    }
}
