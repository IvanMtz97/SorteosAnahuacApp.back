using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SorteoAnahuac.Controllers
{
    /// <summary>
    /// Controlador con métodos para obtener información de los compradores de Sorteo Anáhuac
    /// </summary>

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CompradorController : ApiController
    {
        /// <summary>
        /// Obtiene los datos de un comprador por medio de busqueda.
        /// </summary>
        /// <param name="id">Identidicador del colaborador</param>
        /// <param name="texto">Texto para buscar el comprador</param>
        /// <returns>Estructura de datos con la información básica del comprador solicitado</returns>
        [Code.TokenAuthorize]
        [HttpGet]
        [Route("api/Comprador/Buscar/{id}/{texto}")]
        // GET api/<controller>/5
        public List<Models.Comprador> Buscar(int id, string texto)
        {
            return Models.CompradorService.Obtiene(id, texto);
        }

    }
}
