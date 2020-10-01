using LaShoopa.Models;
using LaShoopa.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LaShoopa.Mocks;

namespace LaShoopa.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationContext _db;
        private readonly IServiceProvider _service;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly string LoginStr = "88186926A85D49ADB762485B47416B1D";
        private readonly string PasswordStr = "DBAF79028EF548F7B8F8D1A328CC3DC6";

        public AdminController(ApplicationContext db, IServiceProvider service, IWebHostEnvironment appEnvironment)
        {
            _db = db;
            _service = service;
            _appEnvironment = appEnvironment;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ISession session = _service.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
                if (string.Equals(model.Login, LoginStr) && string.Equals(model.Password, PasswordStr))
                {
                    session.SetString("LoggedIn", "true");
                    return RedirectToAction("ProductsPage");
                }
                else
                {
                    ModelState.AddModelError("", "Login or password entered incorrectly");
                    session.SetString("LoggedIn", "false");
                }
            }
            return View(model);
        }

        private bool LoggedIn()
        {
            ISession session = _service.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            bool loggedIn = Boolean.Parse(session.GetString("LoggedIn") ?? "false");
            return loggedIn;
        }

        

        [HttpGet]
        public IActionResult Settings()
        {
            if (!LoggedIn())
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Settings(SettingsViewModel model)
        {
            if(model.Settings.CountLatestProducts<0 || model.Settings.CountLatestProducts>100)
            {
                ModelState.AddModelError("", "Count of latest products must be grater than 0 and less then 101");
                return View(model);
            }
            if (model.Settings.CountPopularProducts < 0 || model.Settings.CountPopularProducts > 100)
            {
                ModelState.AddModelError("", "Count of popular products must be grater than 0 and less then 101");
                return View(model);
            }
            if (model.Settings.CountProductsOnPage < 0 || model.Settings.CountProductsOnPage > 100)
            {
                ModelState.AddModelError("", "Count products on page must be grater than 0 and less then 101");
                return View(model);
            }
            if (model.Settings.CountLatestProducts < 0 || model.Settings.CountLatestProducts > 20)
            {
                ModelState.AddModelError("", "Count of brands must be grater than 0 and less then 20");
                return View(model);
            }
            if(model.Img == null)
            {
                ModelState.AddModelError("", "Img isn`t upload");
                return View(model);
            }
            if(!model.Img.FileName.Contains(".jpg") && !model.Img.FileName.Contains(".jpeg") && !model.Img.FileName.Contains(".png"))
            {
                ModelState.AddModelError("", "Incorect format of img");
                return View(model);
            }

            string path = "/img/" + model.Img.FileName;
            using(var fileStream  = new FileStream(_appEnvironment.WebRootPath+path, FileMode.Create))
            {
                model.Img.CopyTo(fileStream);
            }
            AppSetting settings = new AppSetting
            {
                CountBrands = model.Settings.CountBrands,
                CountLatestProducts = model.Settings.CountLatestProducts,
                CountProductsOnPage = model.Settings.CountProductsOnPage,
                CountPopularProducts = model.Settings.CountPopularProducts,
                IntroBackgroundUrl = "img/" + model.Img.FileName
            };

            _db.AppSettings.Add(settings);
            _db.SaveChanges();

            return RedirectToAction("ProductsPage");
        }

        public async Task<IActionResult> ProductsPage()
        {
            if (!LoggedIn())
            {
                return RedirectToAction("Login");
            }

            List<Product> Products = await _db.Products.ToListAsync();
            Dictionary<int, Brand> Brands = await _db.Brands.ToDictionaryAsync(p => p.Id);
            Dictionary<int, Category> Categories = await _db.Categories.ToDictionaryAsync(p => p.Id);
            Dictionary<int, Gender> Genders = await _db.Genders.ToDictionaryAsync(p => p.Id);
            Dictionary<int, string> ImgUrls = new Dictionary<int, string>();
            foreach(var item in Products)
            {
                ImgUrls.Add(item.Id, JsonSerializer.Deserialize<string[]>(item.ImgUrls).FirstOrDefault());
            }

            ProductsPageViewModel model = new ProductsPageViewModel {
                Products = Products,
                Brands = Brands,
                Categories = Categories,
                Genders = Genders,
                ImgUrls = ImgUrls
            };

            return View(model);
        }

        public  void DelFromDb(int id)
        {
            if (LoggedIn())
            {
                Product product = _db.Products.Where(el => el.Id == id).FirstOrDefault();
                if(product!= null)
                {
                    if(_db.Products.Where(el => el.CategoryId == product.CategoryId).Count() == 1)
                    {
                        Category category = _db.Categories.FirstOrDefault(el => el.Id == product.CategoryId);
                        if (category != null)
                        {
                            _db.Categories.Remove(category);
                        }
                    }
                    if (_db.Products.Where(el => el.BrandId == product.BrandId).Count() == 1)
                    {
                        Brand brand = _db.Brands.FirstOrDefault(el => el.Id == product.BrandId);
                        if (brand != null && !string.IsNullOrWhiteSpace(brand.ImgUrl))
                        {                            
                           System.IO.File.Delete($"wwwroot/{brand.ImgUrl}");
                        }
                    }
                    if (_db.Products.Where(el => el.GenderId == product.GenderId).Count() == 1)
                    {
                        Gender gender = _db.Genders.FirstOrDefault(el => el.Id == product.GenderId);
                        if (gender != null)
                        {
                            _db.Genders.Remove(gender);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(product.ImgUrls))
                    {
                        string[] ImgUrls = JsonSerializer.Deserialize<string[]>(product.ImgUrls);
                        foreach(var item in ImgUrls)
                        {
                            System.IO.File.Delete($"wwwroot/{item}");
                        }
                    }
                    _db.Products.Remove(product);
                    _db.SaveChanges();
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            if (!LoggedIn())
            {
                return RedirectToAction("Login");
            }
            List<Brand> brands = await _db.Brands.OrderBy(el => el.Name).ToListAsync();
            List<Category> categories = await _db.Categories.OrderBy(el => el.Name).ToListAsync();
            List<Gender> genders = await _db.Genders.OrderBy(el => el.Name).ToListAsync();
            AddProductsViewModel model = new AddProductsViewModel
            {
                Brands = brands,
                Categories = categories,
                Genders = genders
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(AddProductsViewModel model)
        {
            if (!LoggedIn())
            {
                return RedirectToAction("Login");
            }

            if (string.IsNullOrWhiteSpace(model.Product.Name))
            {
                ModelState.AddModelError("", "Name is null or white space");
            }
            if (model.Product.Price < 0)
            {
                ModelState.AddModelError("", "Price must be greater than 0");
            }

            List<string> Sizes = new List<string>();
            if(model.Sizes != null)
            {
                foreach(var item in model.Sizes)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        Sizes.Add(item); 
                    }
                }
            }

            List<IFormFile> ProductImgs = new List<IFormFile>();
            if (model.ProductImgs == null || model.ProductImgs.Count == 0)
            {
                ModelState.AddModelError("", "Product imgs is null or empty");
            }
            else
            {
                foreach (var item in model.ProductImgs)
                {
                    if (item.FileName.Contains(".jpg") || item.FileName.Contains(".jpeg") || item.FileName.Contains(".png"))
                    {
                        ProductImgs.Add(item);
                    }
                }
                if (ProductImgs.Count() == 0)
                {
                    ModelState.AddModelError("", "Product imgs is empty");
                }
            }
            

            if(model.SelectedBrand.Id == -1 && string.IsNullOrWhiteSpace(model.NewBrand.Name))
            {
                ModelState.AddModelError("", "You have not entered a brand name");
                if(model.BrandImg!=null && !(model.BrandImg.FileName.Contains(".jpg") || model.BrandImg.FileName.Contains(".jpeg") || model.BrandImg.FileName.Contains(".png")))
                {
                    ModelState.AddModelError("", "Brand img isn`t valid");
                }  
            }

            if (model.SelectedCategory.Id == -1 && string.IsNullOrWhiteSpace(model.NewCategory.Name))
            {
                ModelState.AddModelError("", "You have not entered a category name");
            }

            if (model.SelectedGender.Id == -1 && string.IsNullOrWhiteSpace(model.NewGender.Name))
            {
                ModelState.AddModelError("", "You have not entered a gender name");
            }

            if (!ModelState.IsValid)
            {
                List<Brand> brands = await _db.Brands.OrderBy(el => el.Name).ToListAsync();
                List<Category> categories = await _db.Categories.OrderBy(el => el.Name).ToListAsync();
                List<Gender> genders = await _db.Genders.OrderBy(el => el.Name).ToListAsync();
                model.Brands = brands;
                model.Categories = categories;
                model.Genders = genders;
                return View(model);
            }


            Brand brand;
            if(model.SelectedBrand.Id == -1)
            {
                brand = new Brand();
                brand.Name = model.NewBrand.Name;
                if(model.BrandImg != null)
                {
                    string path = "/img/Brands/" + model.BrandImg.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        model.BrandImg.CopyTo(fileStream);
                    }
                    brand.ImgUrl = "img/Brands/"+model.BrandImg.FileName;
                    _db.Brands.Add(brand);
                    _db.SaveChanges();
                    brand = _db.Brands.FirstOrDefault(el => el.Name.Equals(brand.Name));
                }
            }
            else
            {
                brand = _db.Brands.FirstOrDefault(el => el.Id == model.SelectedBrand.Id);
            }

            Category category;
            if(model.SelectedCategory.Id == -1)
            {
                category = new Category();
                category.Name = model.NewCategory.Name;
                _db.Categories.Add(category);
                _db.SaveChanges();
                category = _db.Categories.FirstOrDefault(el => el.Name.Equals(category.Name));
            }
            else
            {
                category = _db.Categories.FirstOrDefault(el => el.Id == model.SelectedCategory.Id);
            }

            Gender gender;
            if (model.SelectedGender.Id == -1)
            {
                gender = new Gender();
                gender.Name = model.NewGender.Name;
                _db.Genders.Add(gender);
                _db.SaveChanges();
                gender = _db.Genders.FirstOrDefault(el => el.Name.Equals(gender.Name));
            }
            else
            {
                gender = _db.Genders.FirstOrDefault(el => el.Id == model.SelectedGender.Id);
            }


            Product product = new Product();
            product.Name = model.Product.Name;
            product.Price = model.Product.Price;
            string[] ImgUrls = new string[ProductImgs.Count()];
            for(int i = 0; i< ProductImgs.Count(); i++)
            {
                string path = "/img/Products/" + ProductImgs[i].FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    ProductImgs[i].CopyTo(fileStream);
                }
                ImgUrls[i] = "img/Products/" + ProductImgs[i].FileName;
            }
            product.ImgUrls = JsonSerializer.Serialize<string[]>(ImgUrls);
            product.IsPopular = model.Product.IsPopular;
            product.Sizes = JsonSerializer.Serialize<string[]>(Sizes.ToArray());
            if (!string.IsNullOrWhiteSpace(model.Product.Country))
            {
                product.Country = model.Product.Country;
            }
            if (!string.IsNullOrWhiteSpace(model.Product.Composition))
            {
                product.Composition = model.Product.Composition;
            }
            if (!string.IsNullOrWhiteSpace(model.Product.Description))
            {
                product.Description = model.Product.Description;
            }
            product.Brand = brand;
            product.BrandId = brand.Id;
            product.Gender = gender;
            product.GenderId = gender.Id;
            product.Category = category;
            product.CategoryId = category.Id;

            _db.Products.Add(product);
            _db.SaveChanges();

            return RedirectToAction("ProductsPage");
        }

        public async Task<IActionResult> Orders()
        {
            if (!LoggedIn())
            {
                return RedirectToAction("Login");
            }
            List<Order> Orders = await _db.Orders.OrderByDescending(el => el.Id).ToListAsync();
            Dictionary<int, List<Product>> Products = new Dictionary<int, List<Product>>();
            Dictionary<int, Dictionary<int, string>> Sizes = new Dictionary<int, Dictionary<int, string>>();
            Dictionary<int, string> ImgUrls = new Dictionary<int, string>();
            Dictionary<int, string> Brands = new Dictionary<int, string>();
            Dictionary<int, string> Categories = new Dictionary<int, string>();
            Dictionary<int, string> Genders = new Dictionary<int, string>();
            foreach(var item in Orders)
            {
                List<Product> OrderProducts = new List<Product>();
                List<OrderSize> OrderSizes = JsonSerializer.Deserialize<List<OrderSize>>(item.ProductsSizes);
                int[] productsId = JsonSerializer.Deserialize<int[]>(item.Products);
                Sizes[item.Id] = new Dictionary<int, string>();
                foreach(var el in productsId)
                {
                    Product product = await _db.Products.FirstOrDefaultAsync(p => p.Id == el);
                    
                    if (product != null)
                    {
                        Brand brand = _db.Brands.FirstOrDefault(p => p.Id == product.BrandId);
                        Category category = _db.Categories.FirstOrDefault(p => p.Id == product.CategoryId);
                        Gender gender = _db.Genders.FirstOrDefault(p => p.Id == product.GenderId);
                        ImgUrls[el] = JsonSerializer.Deserialize<string[]>(product.ImgUrls).FirstOrDefault();
                        OrderProducts.Add(product);
                        OrderSize orderSize = OrderSizes.FirstOrDefault(p => p.ProductId == el);
                        if (orderSize != null)
                        {

                            Sizes[item.Id][el] = orderSize.ProductSize;
                            OrderSizes.Remove(orderSize);
                        }
                        if (brand != null)
                        {
                            Brands[el] = brand.Name;
                        }
                        else
                        {
                            Brands[el] = "";
                        }
                        if (category != null)
                        {
                            Categories[el] = category.Name;
                        }
                        else
                        {
                            Categories[el] = "";
                        }
                        if (gender != null)
                        {
                            Genders[el] = gender.Name;
                        }
                        else
                        {
                            Genders[el] = "";
                        }
                    }
                }
                Products[item.Id] = OrderProducts;
            }


            OrderViewModel model = new OrderViewModel
            {
                Orders = Orders,
                Products = Products,
                ImgUrls = ImgUrls,
                Sizes = Sizes,
                Brands = Brands,
                Genders = Genders,
                Categories = Categories
            };


            return View(model);
        }

        public void DelOrderFromDb(int id)
        {
            if (!LoggedIn())
            {
                return;
            }
            Order order = _db.Orders.FirstOrDefault(el => el.Id == id);
            if (order != null)
            {
                _db.Orders.Remove(order);
                _db.SaveChanges();
            }
        }
    }

}
