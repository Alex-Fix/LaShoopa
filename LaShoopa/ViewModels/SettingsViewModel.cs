using LaShoopa.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaShoopa.ViewModels
{
    public class SettingsViewModel
    {
        public AppSettings Settings { get; set; }
        public IFormFile Img { get; set; }
    }
}
