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
            AppSettings settings = new AppSettings
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

        public async void DelFromDb(int id)
        {
            if (LoggedIn())
            {
                Product product = _db.Products.Where(el => el.Id == id).FirstOrDefault();
                if(product!= null)
                {
                    product.Category = _db.Categories.Where(el => el.Id == id).FirstOrDefault();
                    _db.Products.Remove(product);
                    _db.Categories.Where(el => el.Id == product.CategoryId).FirstOrDefault().Products.Remove(product);
                    _db.Brands.Where(el => el.Id == product.BrandId).FirstOrDefault().Products.Remove(product);
                    _db.Genders.Where(el => el.Id == product.GenderId).FirstOrDefault().Products.Remove(product);
                    await _db.SaveChangesAsync();
                }
            }
        }
    }

}
