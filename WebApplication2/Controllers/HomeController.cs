using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.DBContext;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        MinerDbContext _context;
        private IHostingEnvironment Environment;
        public HomeController(ILogger<HomeController> logger, IHostingEnvironment _environment, MinerDbContext context)
        {
            _logger = logger;
            Environment = _environment;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(IFormFile files)
        {

            var rootPath = Environment.WebRootPath;

            var fullPath = Path.Combine(rootPath, files.FileName);
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                files.CopyTo(stream);
            }

            var Datos = new Dto.File()
            {
                encabezado = new Dto.Encabezado { },
                detalles = new List<Dto.Detalle>() { }
            };

            if (files != null)
            {
                string path = Path.Combine(Environment.WebRootPath, "Nom1");
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
                    if (File != "")
                        if (PrimeraLina)
                        {

                            CultureInfo provider = CultureInfo.InvariantCulture;
                            string[] fila = File.Split('|');
                            Datos.encabezado.Fecha = DateTime.ParseExact(fila[2], "ddMMyyyy", provider);
                            Datos.encabezado.CodigoCebtro = fila[3];
                            Datos.encabezado.CodigoPlantel = fila[4];
                            PrimeraLina = false;

                        }
                        else
                        {
                            var Notas = new List<Dto.Calificaciones>();
                            string[] fila = File.Split('|');

                            foreach (var Calificaciones in fila[8].Split(','))
                            {
                                string[] Calificaion = Calificaciones.Split(':');
                                Notas.Add(new Dto.Calificaciones
                                {
                                    Course = Calificaion[0],
                                    Score = int.Parse(Calificaion[1])
                                });
                            }

                            Datos.detalles.Add(new Dto.Detalle
                            {
                                NumeroDeDocumento = fila[2],
                                Rne = fila[3],
                                Municipio = fila[4],
                                Periodo = fila[5],
                                Nivel = fila[6],
                                Grado = int.Parse(fila[7]),
                                Calificaiones = Notas,
                                Tipodesangre = fila[9],
                                Discapacidad = fila[10]
                            });


                            SaveStudent(new Dto.Detalle
                            {
                                NumeroDeDocumento = fila[2],
                                Rne = fila[3],
                                Municipio = fila[4],
                                Periodo = fila[5],
                                Nivel = fila[6],
                                Grado = int.Parse(fila[7]),
                                Calificaiones = Notas,
                                Tipodesangre = fila[9],
                                Discapacidad = fila[10]
                            });

                            int id = _context.Students.OrderByDescending(t=>t.StudentId).FirstOrDefault().StudentId;

                            foreach (var nota in Notas) 
                            {
                                SaveRating(new Dto.Calificaciones { Course = nota.Course, Score = nota.Score }, id);
                            }

                        }
                }
                
                return View(Datos);
            }
            return View();
        }
        public void SaveStudent(Dto.Detalle Studiante) 
        {
            _context.Students.Add(new Students {
                AcademicLevel = Studiante.Nivel,
                BloodType = Studiante.Tipodesangre,
                ConditionDescription = Studiante.Discapacidad,
                Rne = Studiante.Rne,
                Level = Studiante.Grado,
                Period = Studiante.Periodo,
                State = Studiante.Municipio,
                StudentCode = Studiante.NumeroDeDocumento,

            });
            _context.SaveChanges();
           

        }

        public void SaveRating(Dto.Calificaciones Rating,int id)
        {
            _context.Ratings.Add(new Ratings
            {
               StudentId = id,
               Course = Rating.Course,
               Score = Rating.Score

            });
            _context.SaveChanges();


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
