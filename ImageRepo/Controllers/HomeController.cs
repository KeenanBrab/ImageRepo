using ImageRepo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageRepo.Services;
using MongoDB.Bson;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ImageRepo.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ImageService _imgSvc;
        private readonly UserService _usSvc;
        private readonly IWebHostEnvironment _appEnvironment;

        public HomeController(ILogger<HomeController> logger, ImageService imageService, UserService userService, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _imgSvc = imageService;
            _usSvc = userService;
            _appEnvironment = appEnvironment;
        }

        public async Task<ActionResult> logout()
        {
            HttpContext.Session.Clear();
            
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> login()
        {
            return View();
        }

        public async Task<ActionResult> register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> register(User info)
        {
            try
            {
            HttpContext.Session.SetString("SessionUser", JsonConvert.SerializeObject(info));
            var login = await _usSvc.Create(info);
            return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> login(User info)
        {
            try
            {
                var login = await _usSvc.Login(info.User_Id, info.Password);

                if (login != null)
                {
                    HttpContext.Session.SetString("SessionUser", JsonConvert.SerializeObject(info));
                    return RedirectToAction("Index");
                }

                return View();
            }catch(Exception e)
            {
                _logger.LogInformation(e.Message);
                throw;
            }
        }

        //Returns index repo page, can accept a search paramter to filter result
        public async Task<ActionResult> Index(string search)
        {
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("login");
            }
            try
            {
                var userinfo = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("SessionUser"));
                
                //var userinfo  = new User();
                //userinfo.User_Id = "test";
                
                //var user = await _userManager.GetUserAsync(HttpContext.User);
                var images = await _imgSvc.GetCollection(userinfo.User_Id);

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
        // Adds an image to the users repo, image meta data is stored in mongo db database.
        // Actual image is stored on disk.
        // checks user id and verify image doesnt already exist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Image>> AddImage(InputImage model)
        {
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("login");
            }
            try
            {
                var userinfo = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("SessionUser"));
                
                //var userinfo = new User();
                //userinfo.User_Id = "test";

                await _imgSvc.CheckNameExists(model.Image.FileName, userinfo.User_Id);

                if (await _imgSvc.CheckNameExists(model.Image.FileName, userinfo.User_Id) > 0)
                {
                    return RedirectToAction("Index");
                }

                Image img = new Image();

                img.Description = model.Description;
                img.Name = model.Image.FileName;
            
           
                img.User_Id = userinfo.User_Id;

                var rootFolder = _appEnvironment.WebRootPath;



                img.Path = Path.Combine("Images", userinfo.User_Id, model.Image.FileName);

                var path = Path.Combine(rootFolder, img.Path);


           
                if (!Directory.Exists(Path.Combine(rootFolder, "Images", userinfo.User_Id)))
                {
                    Directory.CreateDirectory(Path.Combine(rootFolder, "Images", userinfo.User_Id));
                }


                using (Stream fileStream = new FileStream(path, FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

            
                await _imgSvc.Create(img);

            }catch(Exception e)
            {
                _logger.LogInformation(e.Message);
                throw;
                
            }
            return RedirectToAction("Index");
        }

        // Deletes an image from both meta data repository and image on disk repository.
        // Since we have two places where we store data we want to make sure both deletes executed successfully
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("login");
            }
            try
            {
                var userinfo = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("SessionUser"));

                //var userinfo = new User();
                //userinfo.User_Id = "test";

                var image = await _imgSvc.Get(id, userinfo.User_Id);

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

                await _imgSvc.Remove(image._id, userinfo.User_Id);
            }
            catch(Exception e)
            {
                _logger.LogInformation(e.Message);
                throw;
            }
            return RedirectToAction("Index");
        }

         //gets an image for the user given a specific image id.
        [HttpGet, ActionName("GetImage")]
        public async Task<ActionResult> GetImage(string id)
        {
            if (HttpContext.Session.GetString("SessionUser") == null)
            {
                return RedirectToAction("login");
            }
            try
            {
                var userinfo = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("SessionUser"));

                //var userinfo  = new User();
                //userinfo.User_Id = "test";

                var image = await _imgSvc.Get(id, userinfo.User_Id);

                if (image == null)
                {
                    return NotFound();
                }


                var rootFolder = _appEnvironment.WebRootPath;
                var path = Path.Combine(rootFolder, image.Path);

                if (System.IO.File.Exists(path))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);

                    return File(fileBytes, "application/force-download", path);

                }
            }catch(Exception e)
            {
                _logger.LogInformation(e.Message);
                throw;
            }
            return NotFound();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
