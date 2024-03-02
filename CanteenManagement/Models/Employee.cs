﻿using System.ComponentModel.DataAnnotations;

namespace CanteenManagement.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public long Contact { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class Owner
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public long Contact { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
