using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SorteoAnahuac.Controllers
{
    /// <summary>
    /// Controlador con métodos para interactuar con talonarios
    /// </summary>
    [Code.TokenAuthorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TalonarioController : ApiController
    {
        /// <summary>
        /// Obtiene los datos de los talonarios asignados al usuario conectado para el sorteo activo actual
        /// </summary>
        /// <returns>Arreglo de talonarios asignados a un usuario</returns>
        // GET api/<controller>
        public IEnumerable<Models.Talonario> Get()
        {
            return Models.TalonarioService.ObtienePorUsuario(User.Identity.Name);
        }

        /// <summary>
        /// Obtiene los datos de un talonario en base a su folio
        /// </summary>
        /// <param name="id">Folio del talonario que se busca obtener</param>
        /// <returns>Estructura de datos con la información del talonario solicitado</returns>
        // GET api/<controller>/5
        public Models.Talonario Get(string id)
        {
            Models.Colaborador persona = Models.ColaboradorService.Obtiene(User.Identity.Name);
            return Models.TalonarioService.Obtiene(id, persona.clave);
        }

        /// <summary>
        /// Acepta los talonarios recibidos en el arreglo de folios
        /// </summary>
        /// <param name="folios">Arreglo de folios de talonarios a aceptar</param>
        [HttpPost]
        [Route("api/Talonario/Aceptar")]
        // Post api/<controller>/Aceptar
        public void Aceptar([FromBody] List<string> folios)
        {
            Models.Colaborador persona = Models.ColaboradorService.Obtiene(User.Identity.Name);
            foreach (string folio in folios)
            {
                Models.TalonarioService.AceptarTalonario(folio, persona.clave);
            };
        }

        /// <summary>
        /// Solicita un nuevo talonario al departamento de sorteos
        /// </summary>
        [HttpPost]
        [Route("api/Talonario/Solicitar")]
        // Post api/<controller>/Solicitar
        public void Solicitar()
        {
            Models.TalonarioService.SolicitarTalonario(User.Identity.Name);
        }
    }
}