using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using testproject.Models;

namespace StoreSystem.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор заказа

        [Required]
        public int CustomerId { get; set; } // Связь с клиентом

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!; // Навигационное свойство

        [Required]
        public int EmployeeId { get; set; } // Связь с сотрудником

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; } = null!; // Навигационное свойство

        [Required]
        public DateTime OrderDate { get; set; } // Дата заказа

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; } // Общая сумма заказа

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>(); // Детали заказа
    }
}
