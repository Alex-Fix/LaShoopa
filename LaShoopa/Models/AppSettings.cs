using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaShoopa.Models
{
    public class AppSettings
    {
        public int Id { get; set; }
        public string IntroBackgroundUrl { get; set; }
        public int CountPopularProducts { get; set; }
        public int CountLatestProducts { get; set; }
        public int CountBrands { get; set; }
        public int CountProductsOnPage { get; set; }
    }
}
