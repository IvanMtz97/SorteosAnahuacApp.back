using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SorteoAnahuac.Controllers
{
    /// <summary>
    /// Controlador que regresa la URL base de los WebAPIs y sitio administrativo del app de Sorteos
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ServerController : ApiController
    {
        /// <summary>
        /// Método que regresa la URL base de los APIs
        /// </summary>
        /// <returns>URL base de los APIs</returns>
        // GET api/<controller>
        public string Get()
        {
            return ConfigurationManager.AppSettings["App.Url.Base"];
        }

    }
}