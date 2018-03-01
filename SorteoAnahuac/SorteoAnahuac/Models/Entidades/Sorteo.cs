using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Entidad que contiene la información de un sorteo. Incluyendo fechas limite, datos de cuenta y ganadores
    /// </summary>
    public class Sorteo
    {
        /// <summary>
        /// Clave única interna del sorteo
        /// </summary>
        public long clave { get; set; }

        /// <summary>
        /// Identificador externo del sorteo
        /// </summary>
        public string identificador { get; set; }

        /// <summary>
        /// Nombre corto del sorteo
        /// </summary>
        public string nombre { get; set; }

        /// <summary>
        /// Descripción larga del sorteo
        /// </summary>
        public string descripcion { get; set; }

        /// <summary>
        /// Fecha de inicio del sorteo
        /// </summary>
        public DateTime fecha_inico { get; set; }

        /// <summary>
        /// Fecha de fin del sorteo
        /// </summary>
        public DateTime fecha_fin { get; set; }

        /// <summary>
        /// Fecha limite para que los colaboradores puedan vender boletos para este sorteo
        /// </summary>
        public DateTime limite_venta { get; set; }

        /// <summary>
        /// Fecha lmite para que los colaboradores puedan depositar pagos para los boletos vendidos
        /// </summary>
        public DateTime limite_abono { get; set; }

        /// <summary>
        /// Cuenta bancaria donde se puede realizar depósitos para pagar los boletos del sorteo
        /// </summary>
        public string cuenta_bancaria { get; set; }

        /// <summary>
        /// URL con información de conoce tu sorteo
        /// </summary>
        public string url_conoce { get; set; }

        /// <summary>
        /// URL con información de tips de venta
        /// </summary>
        public string url_condiciones{ get; set; }

        /// <summary>
        /// URL con información de conoce tu sorteo
        /// </summary>
        public string url_reglamento { get; set; }

        /// <summary>
        /// URL con información de tips de venta
        /// </summary>
        public string url_aceptacion { get; set; }

        /// <summary>
        /// URL con información de Listado de ganadores
        /// </summary>
        public string url_lista_ganadores { get; set; }

        /// <summary>
        /// URL con información de Listado de ganadores
        /// </summary>
        public string url_politicas { get; set; }

        /// <summary>
        /// Listado de boletos ganadores del sorteo
        /// </summary>
        public Ganador[] ganadores { get; set; }
    }
}