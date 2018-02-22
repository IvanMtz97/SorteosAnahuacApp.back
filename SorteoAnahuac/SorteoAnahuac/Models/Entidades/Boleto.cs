using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Estructura de datos que representa un boleto
    /// </summary>
    public class Boleto
    {
        /// <summary>
        /// Clave interna del boleto
        /// </summary>
        public long clave { get; set; }

        /// <summary>
        /// Folio público del boleto
        /// </summary>
        public string folio { get; set; }

        /// <summary>
        /// Token de verificación del boleto
        /// </summary>
        public string token { get
            {
                if (_conToken)
                {
                    return new Code.TokenUtil().Encripta(String.Format("{{\"clave\":\"{0}\",\"folio\":\"{1}\"}}", this.clave, this.folio));
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Folio digital para los boletos vendidos
        /// </summary>
        public string folio_digital { get; set; }

        /// <summary>
        /// Indicador para saber si el boleto ya fue vendido
        /// </summary>
        public bool vendido { get; set; }

        /// <summary>
        /// Costo del boleto
        /// </summary>
        public decimal costo { get; set; }

        /// <summary>
        /// Datos del comprador
        /// </summary>
        public Comprador comprador { get; set; }

        /// <summary>
        /// Folio del talonario al que pertence el boleto
        /// </summary>
        public string folio_talonario { get; set; }

        /// <summary>
        /// Clave del sorteo a la que pertenece el boleto
        /// </summary>
        public long? clave_sorteo;

        /// <summary>
        /// Clave del colaborador que vende el boleto 
        /// </summary>
        public long? clave_colaborador;

        /// <summary>
        /// Indicador de generación de token
        /// </summary>
        bool _conToken;

        /// <summary>
        /// Constructor público default para boletos. Incluey token
        /// </summary>
        public Boleto() : this(true)
        {
        }

        /// <summary>
        /// Constructo de boleto, donde podemos indicar si queremos o no que calcule su token
        /// </summary>
        /// <param name="conToken"></param>
        public Boleto(bool conToken)
        {
            this._conToken = conToken;
        }
    }
}