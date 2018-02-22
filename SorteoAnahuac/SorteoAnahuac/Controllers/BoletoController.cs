using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SorteoAnahuac.Controllers
{
    /// <summary>
    /// Controlador con las acciones relacionadas a la consulta y venta de boletos electrónicos
    /// </summary>
    [Code.TokenAuthorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BoletoController : ApiController
    {

        /// <summary>
        /// Obtiene la información de un boleto por su clave
        /// </summary>
        /// <param name="id">folio del boleto que se desea obtener</param>
        /// <returns>Estructura de datos con la información del boleto</returns>
        // GET api/<controller>/5
        public Models.Boleto Get(int id)
        {
            Models.Colaborador persona = Models.ColaboradorService.Obtiene(User.Identity.Name);
            return Models.TalonarioService.ObtieneBoleto(id, persona.clave);
        }

        /// <summary>
        /// Marca un boleto como vendido sin haber recibido un pago aún
        /// </summary>
        /// <param name="boleto">Estructura de datos que representa el boleto a ser vendido</param>
        /// <returns>Valor indicando el estatus de la venta: 0) No vendido. 1) vendido. 2) vendido previamente.</returns>
        [HttpPost]
        [Route("api/Boleto/Vender")]
        // Post api/<controller>/Vender
        public int Vender([FromBody] Models.BoletoVenta boleto)
        {
            Models.Boleto boletoVenta = new Models.Boleto();
            boletoVenta.clave = long.Parse(boleto.clave);
            boletoVenta.comprador = boleto.comprador;
            boletoVenta.folio = boleto.folio;
            boletoVenta.folio_talonario = boleto.folio_talonario;

            Models.Colaborador persona = Models.ColaboradorService.Obtiene(User.Identity.Name);
            return Models.TalonarioService.VenderBoleto(boletoVenta, persona.clave);
        }

        /// <summary>
        /// Reenvia el correo del boleto
        /// </summary>
        /// <param name="clave">clave del boleto a reenviar el correo</param>
        [HttpGet]
        [Route("api/Boleto/Notificar")]
        // Post api/<controller>/Vender
        public void Notificar(int clave)
        {
            Models.Colaborador persona = Models.ColaboradorService.Obtiene(User.Identity.Name);
            Models.TalonarioService.EnviarBoleto(clave, persona.clave);
        }
    }
}