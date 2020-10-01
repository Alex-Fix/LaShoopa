using LaShoopa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaShoopa.ViewModels
{
    public class OrderViewModel
    {
        public List<Order> Orders { get; set; }
        public Dictionary<int, List<Product>> Products { get; set; }
        public Dictionary<int, string> Brands { get; set; }
        public Dictionary<int, string> Categories { get; set; }
        public Dictionary<int, string> Genders { get; set; }
        public Dictionary<int, Dictionary<int, string>> Sizes { get; set; }
        public Dictionary<int, string> ImgUrls { get; set; }
    }
}
