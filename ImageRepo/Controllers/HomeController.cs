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
using MongoDB.Bson;
using Microsoft.AspNetCore.Hosting;

namespace ImageRepo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ImageService _imgSvc;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _appEnvironment;

        public HomeController(ILogger<HomeController> logger, ImageService imageService, UserManager<IdentityUser> userManager, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _imgSvc = imageService;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }

        
        public async Task<ActionResult> Index(string search)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var images = _imgSvc.GetCollection(user.Id);

                if(search == null)
                {
                    return View(images);
                }
                else
                {
                    List<Image> searchList = new List<Image>();

                    foreach (Image img in images)
                    {

                        if (img.Description.Contains(search) || img.Name.Contains(search))
                        {
                            searchList.Add(img);
                        }
                        
                    }
                    return View(searchList);
                }

               
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                throw;

            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Image>> Index(InputImage model)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            await _imgSvc.CheckNameExists(model.Image.FileName,user.Id);

            if (await _imgSvc.CheckNameExists(model.Image.FileName, user.Id) > 0)
            {
                return RedirectToAction("Index");
            }

            Image img = new Image();

            img.Description = model.Description;
            img.Name = model.Image.FileName;
            
           
            img.User_Id = user.Id;

            var rootFolder = _appEnvironment.WebRootPath;



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
                await _imgSvc.Create(img);

            }catch(Exception e)
            {
                _logger.LogInformation(e.Message);
                throw;
                
            }
            return RedirectToAction("Index");
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            
            var image = _imgSvc.Get(id);

            if (image == null)
            {
                return NotFound();
            }

            var rootFolder = _appEnvironment.WebRootPath;
            var path = Path.Combine(rootFolder, image.Path);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            await _imgSvc.Remove(image._id);

            return RedirectToAction("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
