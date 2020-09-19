using LaShoopa.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LaShoopa.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LaShoopa.Controllers
{
    public class HomeController:Controller
    {
        private ApplicationContext db;

        public HomeController(ApplicationContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            AppSettings setting = db.AppSettings.ToList().LastOrDefault(); 
            List<Product> Products = await db.Products.ToListAsync();

            List<Product> PopularProducts = Products.Where(el => el.IsPopular).ToList();
            if (setting.CountPopularProducts < PopularProducts.Count)
            {
                PopularProducts = PopularProducts.GetRange(0, setting.CountPopularProducts);
            }

            List<Product> LatestProducts;
            if (setting.CountLatestProducts < Products.Count)
            {
                LatestProducts = Products.GetRange(Products.Count - setting.CountLatestProducts, setting.CountLatestProducts);
            }
            else
            {
                LatestProducts = Products;
            }

            List<Brand> Brands = await db.Brands.Where(el => el.ImgUrl != "").ToListAsync();
            if(setting.CountBrands< Brands.Count)
            {
                Brands = Brands.GetRange(0, setting.CountBrands);
            }

            IndexViewModel model = new IndexViewModel {
                PopularProducts = PopularProducts,
                LatestProducts = LatestProducts,
                Brands = Brands,
                IntroBackground = setting.IntroBackgroundUrl
            };
            return View(model);
        }
        
        public IActionResult Product(int id)
        {
            Product Product = db.Products.Where(el => el.Id == id).FirstOrDefault();
            string[] ImgUrls = JsonSerializer.Deserialize<string[]>(Product.ImgUrls);
            string[] Sizes = JsonSerializer.Deserialize<string[]>(Product.Sizes);
            Product.Brand = db.Brands.Where(el => el.Id == Product.BrandId).FirstOrDefault();
            Product.Category = db.Categories.Where(el => el.Id == Product.CategoryId).FirstOrDefault();
            Product.Gender = db.Genders.Where(el => el.Id == Product.GenderId).FirstOrDefault();
            ProductViewModel model = new ProductViewModel
            {
                Product = Product,
                ImgUrls = ImgUrls,
                Sizes = Sizes
            };
            return View(model);
        }
        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Clothes(int genderId = 0, int categoryId = 0, int pageId = 1, int brandId = 0)
        {
            AppSettings setting = await db.AppSettings.FirstOrDefaultAsync();

            List<Product> Products = await db.Products.ToListAsync();
            if (brandId != 0)
            {
                Products = Products.Where(el => el.BrandId == brandId).ToList();
            }
            if (genderId != 0)
            {
                Products = Products.Where(el => el.GenderId == genderId).ToList();
            }
            int CountOfProducts = Products.Count;
            List<Category> Categ = await db.Categories.ToListAsync();
            Category FillCat = Categ.FirstOrDefault(el => el.Id == categoryId);
            Categ = Categ.Where(el => Products.Contains(Products.FirstOrDefault(pr => pr.CategoryId == el.Id))).ToList();
            Dictionary<Category, int> Categories = new Dictionary<Category, int>();
            foreach(var item in Categ)
            {
                Categories.Add(item, Products.Where(el => el.CategoryId == item.Id).Count());
            }
            if (categoryId != 0)
            {
                Products = Products.Where(el => el.CategoryId == categoryId).ToList();
            }
            int countProductsOnPage = (int)Math.Ceiling((double)Products.Count / setting.CountProductsOnPage);
            if (Products.Count > setting.CountProductsOnPage*pageId)
            {
                Products = Products.GetRange(setting.CountProductsOnPage * (pageId-1), setting.CountProductsOnPage);
            }
            else
            {
                Products = Products.GetRange(setting.CountProductsOnPage * (pageId - 1), Products.Count - setting.CountProductsOnPage * (pageId - 1));
            }
            

            ClothesViewModel model = new ClothesViewModel
            {
                Products = Products,
                Categories = Categories,
                CountOfPages = countProductsOnPage,
                FillCat= FillCat,
                countOfProducts = CountOfProducts,
                genderId = genderId,
                pageId = pageId,
                categoryId = categoryId,
                brandId = brandId
            };

            return View(model);
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
