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
using Microsoft.AspNetCore.Identity;

namespace ImageRepo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ImageService _imgSvc;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ImageService imageService, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _imgSvc = imageService;
            _userManager = userManager;
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

            //using (var memoryStream = new MemoryStream())
            //{
                //await model.Image.CopyToAsync(memoryStream);
                //var bytes = memoryStream.ToArray();
                //var hexString = Convert.ToBase64String(bytes);
                //img.Path = hexString;
            //}
            img.Description = model.Description;
            img.Name = model.Image.FileName;

            var user = await _userManager.GetUserAsync(HttpContext.User);
            

            var rootFolder = Directory.GetCurrentDirectory();



            img.Path = Path.Combine("Images", user.Id, model.Image.FileName);

            var path = Path.Combine(rootFolder, img.Path);


           
            if (!Directory.Exists(Path.Combine(rootFolder, "Images", user.Id)))
            {
                Directory.CreateDirectory(Path.Combine(rootFolder, "Images", user.Id));
            }


            using (Stream fileStream = new FileStream(path, FileMode.Create))
            {
                await model.Image.CopyToAsync(fileStream);
            }

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
