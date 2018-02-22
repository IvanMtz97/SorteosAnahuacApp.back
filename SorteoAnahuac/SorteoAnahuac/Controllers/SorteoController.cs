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
    /// Controlador con métodos para interactuar con sorteos
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SorteoController : ApiController
    {
        /// <summary>
        /// Obtiene la información de un sorteo en base a su clave de identificación interna
        /// </summary>
        /// <param name="id">Identificador único del sorteo que se desea obtener</param>
        /// <returns></returns>
        // GET api/<controller>/5
        public Models.Sorteo Get(long id)
        {
            return Models.SorteoService.Obtener(id);
        }

        /// <summary>
        /// Método que permite obtener la información del sorteo activo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Sorteo/Activo")]
        // GET api/<controller>/5
        public Models.Sorteo Activo()
        {
            return Models.SorteoService.ObtenerActivo();
        }
    }
}