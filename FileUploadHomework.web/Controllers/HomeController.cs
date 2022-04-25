using FileUploadHomework.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FileUploadHomework.data;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;

namespace FileUploadHomework.web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=FileUploads;Integrated Security=True";

        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(IWebHostEnvironment webHostEnvironment) { _webHostEnvironment = webHostEnvironment; }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(Image image, IFormFile imageFile)
        {
            string fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            imageFile.CopyTo(fs);

            image.ImagePath = filePath;

            FileUploadRepository db = new FileUploadRepository(_connectionString);
    
            image.Id = db.AddImage(image);
            image.Password = db.GetPasswordByImageId(image.Id);

            FileUploadViewModel vm = new FileUploadViewModel
            {
                Image = image
            };
            return View(vm);
        }

        //public IActionResult Upload(Image image)
        //{
        //    return View();
        //}

        public IActionResult ViewImage(string password, int id)
        {
            FileUploadRepository db = new FileUploadRepository(_connectionString);

            bool isAllowed = false;

            var imageIds = HttpContext.Session.Get<List<int>>("IdsList");

            if(imageIds != null && imageIds.Contains(id))
            {
                isAllowed = true;
                db.UpdateView(id);
            }

            if (imageIds == null)
            {
                imageIds = new List<int>();
            }

            //if (!imageIds.Contains(id))
            //{
            //    return Redirect($"/home/enterpassword?id={id}");
            //}

            if (!String.IsNullOrEmpty(password) && password == db.GetPasswordByImageId(id))
            {
                imageIds.Add(id);
                HttpContext.Session.Set("IdsList", imageIds);
            }
            else
            {
                TempData["message"] = "Invalid Password";
                return Redirect($"/home/enterpassword?id={id}");
            }

            Image image = db.GetImageById(id);
            db.UpdateView(id);

            return View(new FileUploadViewModel
            {
                Image = image,
                IsAllowed = isAllowed
            });

        }

        public IActionResult EnterPassword(int id)
        {
            var imageIds = HttpContext.Session.Get<List<int>>("IdsList");

            if (imageIds != null && imageIds.Contains(id))
            {
                return Redirect($"/home/viewImage?id={id}");
            }

            return View(new PasswordViewModel
            {
                Id = id,
                IncorrectPassword = (string)TempData["incorrectPassword"]
            });
        }

    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }

}



