using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; 
        public ICollection<Employee> Employees { get; set; } = new List<Employee>(); 
    }


}
