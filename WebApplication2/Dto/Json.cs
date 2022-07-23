using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Dto
{
    public class Json
    {
        public string TipoRegistro { get; set; }
        public string FechaRemision { get; set; }
        public string CodigoCentro { get; set; }
        public string CodigoPlantel { get; set; }

        public string CodigoCentroMinerd { get; set; }

        public string Rne { get; set; }
        public string NivelAcademico { get; set; }
        public string PeriodoAcademico { get; set; }
        public string GradoAcademico { get; set; }
        public string InformacionAcademica { get; set; }

        public string MunicipioResidencia { get; set; }

        public string TipodeSangre { get; set; }
        public string CondicionesDiscapacidad { get; set; }

    }
}
