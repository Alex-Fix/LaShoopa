using LaShoopa.Models;
using LaShoopa.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LaShoopa.Controllers
{
    public class ShopCartController : Controller
    {
        private  ShopCart _shopCart;
        private readonly ApplicationContext _db;
        private readonly IServiceProvider _service;
        public ShopCartController(ShopCart shopCart, ApplicationContext db, IServiceProvider service)
        {
            _shopCart = shopCart;
            _db = db;
            _service = service;
        }

        public string ViewOrders()
        {
            Order order = _db.Orders.ToList().LastOrDefault();
            if (order != null)
            {
                return order.Products;
            }
            return "null";
        }

        [HttpGet]
        public IActionResult Cart()
        {
            var items = _shopCart.GetShopItems();
            _shopCart.listShopItems = items;
            Dictionary<int, string> ImgUrls = new Dictionary<int, string>();
            foreach(var item in items)
            {
                ImgUrls[item.product.Id] = JsonSerializer.Deserialize<string[]>(item.product.ImgUrls).FirstOrDefault();
            }
            var model = new ShopCartViewModel
            {
                shopCart = _shopCart,
                ImgUrls = ImgUrls
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cart(ShopCartViewModel model)
        {
            if (ModelState.IsValid)
            {
                Order order = model.Order;
                order.Products = JsonSerializer.Serialize<int[]>(_shopCart.GetShopItems().Select(el => el.product.Id).ToArray());
                order.Price = _shopCart.GetShopItems().Sum(el => el.product.Price);
                Dictionary<string, string> Sizes = new Dictionary<string, string>();
                foreach(var item in _shopCart.GetShopItems())
                {
                    Sizes[item.product.Id.ToString()] = item.Size;
                }
                order.ProductsSizes = JsonSerializer.Serialize<Dictionary<string, string>>(Sizes);
                _db.Orders.Add(order);
                _db.SaveChanges();
                _shopCart = ShopCart.CreateNewCart(_service);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(model);
            }
        }

        public void AddToCart(int id, string size = "")
        {

            var item = _db.Products.FirstOrDefault(el => el.Id == id);
            if(item != null)
            {
                 _shopCart.AddToCart(item, size);
            }
        }

        public void DelFromCart(int id)
        {
            var item = _db.Products.FirstOrDefault(el => el.Id == id);
            if(item != null)
            {
                _shopCart.DelFromCart(item);
            }
        }

    }
}
