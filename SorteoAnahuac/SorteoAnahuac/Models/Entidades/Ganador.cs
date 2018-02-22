using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Estructura de datos que representa un ganador
    /// </summary>
    public class Ganador
    {
        /// <summary>
        /// Lugar que obtuvo el boleto
        /// </summary>
        public int lugar { get; set; }
        /// <summary>
        /// Folio del boleto ganador
        /// </summary>
        public string folio { get; set; }
    }
}