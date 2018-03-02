using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Estructura de datos que representa los datos personales de un comprador de un boleto
    /// </summary>
    public class Comprador
    {
        /// <summary>
        /// Nombre de la persona que compró el boleto
        /// </summary>
        public string nombre { get; set; }

        /// <summary>
        /// Apellidos del comprador
        /// </summary>
        public string apellidos { get; set; }

        /// <summary>
        /// Nombre completo del comprador
        /// </summary>
        public string nombre_completo
        {
            get
            {
                return String.Format("{0} {1}", this.nombre, this.apellidos).Trim().Replace("  ", " ");
            }
        }

        /// <summary>
        /// Correo electrónico del comprador
        /// </summary>
        public string correo { get; set; }

        /// <summary>
        /// Número de celular del comprador
        /// </summary>
        public string celular { get; set; }

        /// <summary>
        /// Dirección del comprador
        /// </summary>
        public Direccion direccion { get; set; }

    }
}