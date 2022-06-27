using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Dto
{
    public class Detalle
    {

        public string NumeroDeDocumento { get; set; }

        public string Rne { get; set; }
        public string Nivel { get; set; }
        public string Periodo { get; set; }
        public int Grado { get; set; }
        
         public string Municipio { get; set; }

        public List<Calificaciones> Calificaiones { get; set; }

        public string Tipodesangre { get; set; }
        public string Discapacidad { get; set; }

    }
}
