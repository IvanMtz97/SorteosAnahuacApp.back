using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Estructura que representa un talonario digital
    /// </summary>
    public class Talonario
    {
        /// <summary>
        /// Identificador interno único del talonario
        /// </summary>
        public long clave { get; set; }

        /// <summary>
        /// Folio de indentificación del talonario
        /// </summary>
        public string folio { get; set; }

        /// <summary>
        /// Listado de boletos asignados al talonario ditital
        /// </summary>
        public Boleto[] boletos { get; set; }

        /// <summary>
        /// Diccionario de boletos al talonario ditital
        /// </summary>
        public Dictionary<string, Array> Boletos = new Dictionary<string, Array>();
    }
}