using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Estructura de datos que representa un colaborador
    /// </summary>
    public class Colaborador
    {
        public long clave { get; set; }
        public string identificador { get; set; }
        public string nombre { get; set; }
        public string apellido_paterno { get; set; }
        public string apellido_materno { get; set; }

        string _nombreCompleto;

        public string nombre_completo { get
            {
                if (!_calculaNombre)
                {
                    return _nombreCompleto;
                }
                return String.Format("{0} {1} {2}", this.nombre, this.apellido_paterno, this.apellido_materno).Trim();
            }
        }
        public string correo { get; set; }
        public DateTime expira { get; set; }
        public string token { get; set; }
        public Talonario[] talonarios { get; set; }
        public string referencia_bancaria { get; set; }
        public decimal monto_total { get; set; }
        public decimal monto_abonado { get; set; }
        public decimal monto_deudor { get; set; }
        public string version { get; set; }

        bool _calculaNombre;

        public Colaborador() : this(true, null) { }

        public Colaborador(bool calculaNombre, string nombre)
        {
            this._calculaNombre = false;
            this._nombreCompleto = nombre;
        }

        public string modelo_telefono { get; set; }
    }
}