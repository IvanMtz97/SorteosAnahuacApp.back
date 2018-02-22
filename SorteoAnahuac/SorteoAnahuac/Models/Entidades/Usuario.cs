using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Clase que representa los datos de un usuario del sistema
    /// </summary>
    public class Usuario
    {
        public long clave { get; set; }
        public string identificador { get; set; }
        public string nombre { get; set; }
        public string apellido_paterno { get; set; }
        public string apellido_materno { get; set; }
        public string nombre_completo { get
            {
                return String.Format("{0} {1} {2}", this.nombre, this.apellido_paterno, this.apellido_materno).Trim().Replace("  ", " ");
            }
        }
        public string correo { get; set; }
        public DateTime expira { get; set; }
        public string token { get; set; }
    }
}