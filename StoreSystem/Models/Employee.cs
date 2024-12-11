using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace StoreSystem.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор сотрудника

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!; // Имя

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!; // Фамилия

        [Required]
        public int PostId { get; set; } // Связь с должностью

        [ForeignKey("PostId")]
        public Post Post { get; set; } = null!; // Навигационное свойство

        [Required]
        public DateTime HireDate { get; set; }= DateTime.Now;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; } // Зарплата
    }
}
