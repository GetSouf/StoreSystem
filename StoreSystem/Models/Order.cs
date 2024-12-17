using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using testproject.Models;

namespace StoreSystem.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public int CustomerId { get; set; } 

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!; 

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; } = null!; 

        [Required]
        public DateTime OrderDate { get; set; }  

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; } 

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public string Status { get; set; } = "Created";
    }

}
