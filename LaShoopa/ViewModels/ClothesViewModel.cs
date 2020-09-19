using LaShoopa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaShoopa.ViewModels
{
    public class ClothesViewModel
    {
        public List<Product> Products { get; set; }
        public Dictionary<Category, int> Categories { get; set; }
        public int CountOfPages { get; set; }
        public Category FillCat { get; set; }
        public int countOfProducts { get; set; }
        public int genderId { get; set; }
        public int pageId { get; set; }
        public int categoryId { get; set; }
        public int brandId { get; set; }
        
    }
}
