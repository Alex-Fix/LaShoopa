using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LaShoopa.Models
{
    public class ShopCartItem
    {
        [Key]
        public int itemId { get; set; }
        public Product product { get; set; }
        public string Size { get; set; }
        public string ShopCartId { get; set; }
    }
}
