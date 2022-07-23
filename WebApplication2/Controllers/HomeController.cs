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
using System.Net;
using Newtonsoft.Json;
using WebApplication2.DBContext;
using WebApplication2.Models;
using WebApplication2.Dto;

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
        public IActionResult Index(IFormFile files, string url = "")
        {

            var rootPath = Environment.WebRootPath;
            var Datos = new Dto.File()
            {
                encabezado = new Dto.Encabezado { },
                detalles = new List<Dto.Detalle>() { }
            };
            if (files != null)
            {
                var fullPath = Path.Combine(rootPath, files.FileName);
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    files.CopyTo(stream);
                }




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

                            int id = _context.Students.OrderByDescending(t => t.StudentId).FirstOrDefault().StudentId;

                            foreach (var nota in Notas)
                            {
                                SaveRating(new Dto.Calificaciones { Course = nota.Course, Score = nota.Score }, id);
                            }

                        }
                }

                return View(Datos);
            }
            else if (url != "")
            {

                var csvData = Get(url);
                bool PrimeraLina = true;
                foreach (var File in csvData)
                {
                    if (File != null)
                        if (PrimeraLina)
                        {

                            CultureInfo provider = CultureInfo.InvariantCulture;

                            Datos.encabezado.Fecha = DateTime.ParseExact(File.FechaRemision, "ddMMyyyy", provider);
                            Datos.encabezado.CodigoCebtro = File.CodigoCentro;
                            Datos.encabezado.CodigoPlantel = File.CodigoPlantel;
                            PrimeraLina = false;

                        }
                        else
                        {
                            var Notas = new List<Dto.Calificaciones>();


                            foreach (var Calificaciones in File.InformacionAcademica.Split(','))
                            {
                                if (Calificaciones != "") {
                                string[] Calificaion = Calificaciones.Split(':');
                                Notas.Add(new Dto.Calificaciones
                                {
                                    Course = Calificaion[0],
                                    Score = int.Parse(Calificaion[1])
                                });
                                }
                            }

                            Datos.detalles.Add(new Dto.Detalle
                            {
                                NumeroDeDocumento = File.CodigoCentroMinerd,
                                Rne = File.Rne,
                                Municipio = File.MunicipioResidencia,
                                Periodo = File.PeriodoAcademico,
                                Nivel = File.NivelAcademico,
                                Grado = int.Parse(File.GradoAcademico),
                                Calificaiones = Notas,
                                Tipodesangre = File.TipodeSangre,
                                Discapacidad = File.CondicionesDiscapacidad
                            });


                            SaveStudent(new Dto.Detalle
                            {
                                NumeroDeDocumento = File.CodigoCentroMinerd,
                                Rne = File.Rne,
                                Municipio = File.MunicipioResidencia,
                                Periodo = File.PeriodoAcademico,
                                Nivel = File.NivelAcademico,
                                Grado = int.Parse(File.GradoAcademico),
                                Calificaiones = Notas,
                                Tipodesangre = File.TipodeSangre,
                                Discapacidad = File.CondicionesDiscapacidad
                            });

                            int id = _context.Students.OrderByDescending(t => t.StudentId).FirstOrDefault().StudentId;

                            foreach (var nota in Notas)
                            {
                                SaveRating(new Dto.Calificaciones { Course = nota.Course, Score = nota.Score }, id);
                            }

                        }
                }

                return View(Datos);
            }
            else if (files == null && url == "")
            {

                string csvData = Ftprecive();
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

                            int id = _context.Students.OrderByDescending(t => t.StudentId).FirstOrDefault().StudentId;

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
            _context.Students.Add(new Students
            {
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

        public void SaveRating(Dto.Calificaciones Rating, int id)
        {
            _context.Ratings.Add(new Ratings
            {
                StudentId = id,
                Course = Rating.Course,
                Score = Rating.Score

            });
            _context.SaveChanges();


        }
        private string Ftprecive()
        {
            const string site = "ftp://files.000webhost.com//";
            string NombreArchivo = $"LICEO ROSS.txt";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(site + @"\" + NombreArchivo);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.KeepAlive = true;
            request.UsePassive = false;
            request.UseBinary = false; // use true for .zip file or false for a text file

            request.Credentials = new NetworkCredential("niels1709", "niels1709");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            using (FileStream writer = new FileStream(NombreArchivo, FileMode.Create))
            {
                long length = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[2048];

                readCount = responseStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    writer.Write(buffer, 0, readCount);
                    readCount = responseStream.Read(buffer, 0, bufferSize);
                }
            }

            reader.Close();
            response.Close();

            DeleteFile();

            var rootPath = $@"E:\Users\Niels\source\repos\WebApplication2\WebApplication2\"; //get the root path

            var fullPath = Path.Combine(rootPath, NombreArchivo);

            var Data = System.IO.File.ReadAllText(fullPath);
            return Data;
        }

        private void DeleteFile()
        {
            const string site = "ftp://files.000webhost.com//";
            string NombreArchivo = $"LICEO ROSS.txt";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(site + @"\" + NombreArchivo);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new NetworkCredential("niels1709", "niels1709");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();


        }

        private List<Json> Get(string url)
        {


            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

                request.Method = "POST";
                string data = String.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    dataStream.Close();
                }

                #region Prueba_JSON
                /*data = @"[{""tipoRegistro"": ""E"",""fechaRemision"": ""02072022"",""codigoCentro"": ""00639"",""codigoPlantel"": ""19333469"",""numeroRegistros"": ""2""},{""tipoRegistro"": ""D"",""codigoCentroMinerd"": ""1"",""rne"": ""CBD0406190000"",""municipioResidencia"": ""SANTO DOMINGO ESTE"",""periodoAcademico"": ""P1"",""nivelAcademico"": ""S"",""gradoAcademico"": ""4"",""informacionAcademica"": ""LIT:90,MAT:87,NAT:93,SOC:70"",""tipoSangre"" : ""O+"",""condicionesDiscapacidad"" : ""NINGUNA""},{""tipoRegistro"": ""D"",""codigoCentroMinerd"": ""2"",""rne"": ""HUD0107200000"",""municipioResidencia"": ""SANTO DOMINGO ESTE"",""periodoAcademico"": ""P1"",""nivelAcademico"": ""S"",""gradoAcademico"": ""4"",""informacionAcademica"": ""LIT:98,MAT:87,NAT:77,SOC:88"",""tipoSangre"" : ""O+"",""condicionesDiscapacidad"" : ""NINGUNA""}]";*/
                #endregion
                var Datos = JsonConvert.DeserializeObject<List<Json>>(data);
                return Datos;
            }

            catch (Exception ex)
            {
                throw ex;
            }
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
