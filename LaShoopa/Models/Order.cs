using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LaShoopa.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Incorrect phone number")]
        public string Phone { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Length must be greater then 3")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Incorrect email")]
        public string Email { get; set; }
        public string Comment { get; set; }
        public string Products { get; set; }
        public int Price { get; set; }
        public string ProductsSizes { get; set; }
    }
}
