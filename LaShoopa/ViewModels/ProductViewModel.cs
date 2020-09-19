using LaShoopa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaShoopa.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public string[] ImgUrls { get; set; }
        public string[] Sizes { get; set; }
    }
}
