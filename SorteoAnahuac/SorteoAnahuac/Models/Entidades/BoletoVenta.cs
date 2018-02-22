using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Estrucutra de datos para recibir los parametros de venta de un boleto
    /// </summary>
    public class BoletoVenta
    {
        /// <summary>
        /// Clave del boleto a vender
        /// </summary>
        public string clave { get; set; }

        /// <summary>
        /// Folio publico del boleto a vender
        /// </summary>
        public string folio { get; set; }

        /// <summary>
        /// Datos de comprador del boleto
        /// </summary>
        public Comprador comprador { get; set; }

        /// <summary>
        /// Folio del talonario al cual pertenece el boleto
        /// </summary>
        public string folio_talonario { get; set; }
    }

}