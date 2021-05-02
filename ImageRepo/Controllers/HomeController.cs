using ImageRepo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageRepo.Services;

namespace ImageRepo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ImageService _imgSvc;

        public HomeController(ILogger<HomeController> logger, ImageService imageService)
        {
            _logger = logger;
            _imgSvc = imageService;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Image>> Index(InputImage model)
        {
            Image img = new Image();

            using (var memoryStream = new MemoryStream())
            {
                await model.Image.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                var hexString = Convert.ToBase64String(bytes);
                img.ContentImage = hexString;
            }
            img.Description = model.Description;
            img.Name = model.Name;

            try
            {
                _imgSvc.Create(img);

            }catch(Exception e)
            {
                _logger.LogInformation(e.Message);
                
            }
            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
