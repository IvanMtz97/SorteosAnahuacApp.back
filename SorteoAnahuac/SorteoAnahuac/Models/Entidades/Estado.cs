using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SorteoAnahuac.Models.Ubicaciones
{
    public class Estado
    {
        public string nombre { get; set; }
        public Municipio[] municipios { get; set; }
    }
}