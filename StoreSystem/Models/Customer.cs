using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using StoreSystem.Models;


namespace testproject.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!; 

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!; 

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!; 

        [MaxLength(15)]
        public string? Phone { get; set; } 

        [MaxLength(200)]
        public string? Address { get; set; } 

        [Required]

        public DateTime RegistrationDate { get; set; } 



        public ICollection<Order> Orders { get; set; } = new List<Order>();



    }
}
