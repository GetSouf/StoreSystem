using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор должности

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; // Название должности

        public ICollection<Employee> Employees { get; set; } = new List<Employee>(); // Сотрудники на должности
    }


}
