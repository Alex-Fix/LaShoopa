using LaShoopa.Models;
using LaShoopa.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaShoopa.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationContext _db;
        private readonly IServiceProvider _service;
        private readonly string LoginStr = "88186926A85D49ADB762485B47416B1D";
        private readonly string PasswordStr = "DBAF79028EF548F7B8F8D1A328CC3DC6";

        public AdminController(ApplicationContext db, IServiceProvider service)
        {
            _db = db;
            _service = service;
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

        public IActionResult ProductsPage()
        {
            if (!LoggedIn())
            {
                return RedirectToAction("Login");
            }
            return View();
        }

    }
}
