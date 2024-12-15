using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace StoreSystem.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public int PostId { get; set; }

        [ForeignKey("PostId")]
        public Post Post { get; set; }

        [Required]
        public DateTime HireDate { get; set; } = DateTime.Now;

        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Bonus { get; set; } = 0; // Новое поле для бонусов
    }
}
