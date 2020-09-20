using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaShoopa.Models;

namespace LaShoopa.ViewModels
{
    public class IndexViewModel
    {
        public List<Product> PopularProducts { get; set; }
        public List<Product> LatestProducts { get; set; }
        public List<Brand> Brands { get; set; }
        public Dictionary<int, string> ImgUrls { get; set; }
        public string IntroBackground { get; set; }
    }
}
