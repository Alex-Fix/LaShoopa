using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LaShoopa.Models
{
    public class ShopCart
    {
        private readonly ApplicationContext _db;
        public ShopCart(ApplicationContext db)
        {
            _db = db;
        }
        public string ShopCartId { get; set; }
        public List<ShopCartItem> listShopItems { get; set; }

        public static ShopCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<ApplicationContext>();
            string shopCartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", shopCartId);
            return new ShopCart(context){ ShopCartId = shopCartId };
        }

        public static ShopCart CreateNewCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<ApplicationContext>();
            string shopCartId = Guid.NewGuid().ToString();
            session.SetString("CartId", shopCartId);
            return new ShopCart(context) { ShopCartId = shopCartId };
        }

        public void AddToCart(Product product, string size)
        {
            if (string.IsNullOrWhiteSpace(size))
            {
                size = JsonSerializer.Deserialize<string[]>(product.Sizes).FirstOrDefault();
            }
            _db.ShopCartItem.Add(new ShopCartItem
            {
                ShopCartId = ShopCartId,
                product = product,
                Size = size
            }) ;

            _db.SaveChanges();
        }

        public void DelFromCart(Product product)
        {
            var item = _db.ShopCartItem.FirstOrDefault(el => el.product.Id == product.Id && el.ShopCartId == ShopCartId);
            if(item != null)
            {
                _db.ShopCartItem.Remove(item);
            }
            _db.SaveChanges();
        }

        public List<ShopCartItem> GetShopItems()
        {
            return _db.ShopCartItem.Where(el => el.ShopCartId == ShopCartId).Include(el => el.product).ToList();
        }

    }
}
