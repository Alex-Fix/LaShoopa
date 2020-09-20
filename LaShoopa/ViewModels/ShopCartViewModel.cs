using LaShoopa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaShoopa.ViewModels
{
    public class ShopCartViewModel
    {
        public ShopCart shopCart { get; set; }
        public Dictionary<int, string> ImgUrls { get; set; }
        public Order Order { get; set; }
    }
}
