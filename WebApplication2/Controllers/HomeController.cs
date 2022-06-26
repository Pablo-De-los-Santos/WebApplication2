using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IHostingEnvironment Environment;
        public HomeController(ILogger<HomeController> logger, IHostingEnvironment _environment)
        {
            _logger = logger;
            Environment = _environment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(IFormFile files)
        {

            var rootPath = Environment.WebRootPath; //get the root path

            var fullPath = Path.Combine(rootPath, files.FileName); //combine the root path with that of our json file inside mydata directory
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                files.CopyTo(stream);
            }

            var Datos = new Dto.File();

            if (files != null)
            {
                string path = Path.Combine(this.Environment.WebRootPath, "Nom1");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileName(files.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    files.CopyTo(stream);
                }
                string csvData = System.IO.File.ReadAllText(filePath);
                bool PrimeraLina = true;
                foreach (var File in csvData.Split('\n'))
                {

                        if (PrimeraLina) 
                        {
                            string[] fila = File.Split('|');
                            Datos.encabezado.CodigoCebtro = fila[1];
                        }

                }

                return View();
            }
            return View();
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
