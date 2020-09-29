using LaShoopa.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaShoopa.ViewModels
{
    public class AddProductsViewModel
    {
        public List<Brand> Brands { get; set; }
        public List<Gender> Genders { get; set; }
        public List<Category> Categories { get; set; }
        public List<string> Sizes { get; set; }
        public Product Product { get; set; }
        public Brand SelectedBrand { get; set; }
        public Brand NewBrand { get; set; }
        public Gender SelectedGender { get; set; }
        public Gender NewGender { get; set; }
        public Category SelectedCategory { get; set; }
        public Category NewCategory { get; set; }
        public IFormFile BrandImg { get; set; }
        public List<IFormFile> ProductImgs { get; set; }
    }
}
