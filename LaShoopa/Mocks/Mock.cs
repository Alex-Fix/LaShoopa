using LaShoopa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LaShoopa.Mocks
{
    public class Mock: IMock
    {
        private readonly ApplicationContext _db;

        public Mock(ApplicationContext db)
        {
            _db = db;
            if (!db.Products.Any())
            {
                FillDB();
            }
        }

        public void FillDB()
        {
            
            List<Gender> Genders = new List<Gender>
            {
                new Gender
                {
                    Name = "Мужчинам"
                },
                new Gender
                {
                    Name= "Женщинам"
                }
            };
            _db.Genders.AddRange(Genders);

            List<Brand> Brands = new List<Brand>
            {
                new Brand
                {
                    Name = "N.Family",
                    ImgUrl = "img/Brands/1.png"
                },
                new Brand
                {
                    Name = "Arber",
                    ImgUrl = "img/Brands/2.jpg"
                }
            };
            _db.AddRange(Brands);

            List<Category> Categories = new List<Category>
            {
                new Category
                {
                    Name = "Платья"
                },
                new Category
                {
                    Name  = "Куртки"
                }
            };
            _db.AddRange(Categories);

            List<Product> Products = new List<Product>
            {
                new Product
                {
                    Name  = "Платье синее N.Family",
                    Price = 429,
                    IsPopular  = true,
                    Sizes = JsonSerializer.Serialize<string[]>(new string[]{"42", "44", "48", "50", "52", "54"}),
                    Country  = "Украина",
                    Composition = "70% хлопок, 30% вискоза",
                    ImgUrls  = JsonSerializer.Serialize<string[]>(new string[]{"img/Products/1.jpg", "img/Products/2.jpg", "img/Products/3.jpg" }),
                    Description  = "Платье прямого силуэта из хлопкового материала. Изделие с круглой горловиной и коротким рукавом. Модель дополнена принтом.",
                    Brand = Brands[0],
                    Category = Categories[0],
                    Gender = Genders[0]
                },
                new Product
                {
                    Name  = "Куртка черная Arber",
                    Price = 559,
                    IsPopular  = true,
                    Sizes = JsonSerializer.Serialize<string[]>(new string[]{"46-5", "48-5", "50-5", "54-5", "56-5", "58-5"}),
                    Country  = "Китай",
                    Composition = "100% полиэстер",
                    ImgUrls  = JsonSerializer.Serialize<string[]>(new string[]{"img/Products/4.jpg", "img/Products/5.jpg", "img/Products/6.jpg" }),
                    Description  = "Наполнитель: 100% полиэстер.",
                    Brand = Brands[1],
                    Category = Categories[1],
                    Gender = Genders[1]
                }
            };
            _db.AddRange(Products);
            _db.SaveChanges();
        }
    }
}
