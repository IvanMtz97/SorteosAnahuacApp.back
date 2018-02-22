using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SorteoAnahuac.Models
{
    public class Direccion
    {
        public string calle { get; set; }
        public string numero { get; set; }
        public string colonia { get; set; }
        public string codigo_postal { get; set; }
        public string estado { get; set; }
        public string municipio { get; set; }
        public string telefono { get; set; }
    }
}