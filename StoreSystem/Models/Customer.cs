﻿using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using StoreSystem.Models;


namespace testproject.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; } // Уникальный идентификатор клиента

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!; // Имя

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!; // Фамилия

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!; // Электронная почта

        [MaxLength(15)]
        public string? Phone { get; set; } // Телефон

        [MaxLength(200)]
        public string? Address { get; set; } // Адрес

        [Required]
        public DateTime RegistrationDate { get; set; } // Дата регистрации

        public ICollection<Order> Orders { get; set; } = new List<Order>(); // Связь с заказами

    }
}