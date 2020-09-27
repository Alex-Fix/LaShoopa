using LaShoopa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaShoopa.ViewModels
{
    public class ProductsPageViewModel
    {
        public List<Product> Products { get; set; }
        public Dictionary<int, Category> Categories { get; set; }
        public Dictionary<int , Gender> Genders { get; set; }
        public Dictionary<int, Brand> Brands { get; set; }
        public Dictionary<int, string> ImgUrls { get; set; }
    }
}
