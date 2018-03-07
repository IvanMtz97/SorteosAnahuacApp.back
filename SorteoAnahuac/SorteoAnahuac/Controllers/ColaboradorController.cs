using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SorteoAnahuac.Controllers
{
    /// <summary>
    /// Controlador con métodos para obtener información de los colaboradores de Sorteo Anáhuac
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ColaboradorController : ApiController
    {
        /// <summary>
        /// Permite realizar la autenticación de un colaborador, obtener sus datos y un token de sesión
        /// </summary>
        /// <returns>Estructura de datos con la información básica del colaborador autenticado.</returns>
        [Code.BasicAuthorize]
        [Route("api/Colaborador/{uuid}")]
        // GET api/<controller>
        public Models.Colaborador Get(string uuid)
        {
            string correo = User.Identity.Name;

            // Obtenemos los datos del usuario
            Models.Colaborador usuario = Models.ColaboradorService.ObtieneConTalonarios(correo);
            

            //Generamos un token para este usuario
            string token = Models.SessionService.GeneraToken(correo, uuid);
            usuario.token = token;
            usuario.expira = DateTime.UtcNow.AddMinutes(int.Parse(ConfigurationManager.AppSettings["Seguridad.Expiracion"]));
            
            return usuario;
        }

        /// <summary>
        /// Obtiene los datos de un colaborador por su cuenta de correo.
        /// </summary>
        /// <param name="correo">Cuenta de correo del colaborador</param>
        /// <returns>Estructura de datos con la información básica del colaborador solicitado</returns>
        [Code.TokenAuthorize]
        [Route("api/Colaborador/GetCorreo/{correo}")]
        // GET api/<controller>/5
        public Models.Colaborador GetCorreo(string correo)
        {
            if (!(User.Identity.Name == correo))
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.Forbidden);
            }

            return Models.ColaboradorService.ObtieneConTalonarios(correo);
            //return Models.ColaboradorService.Obtiene(correo);
        }

        /// <summary>
        /// Obtiene los datos de un colaborador por su cuenta de correo.
        /// </summary>
        /// <param name="clave">Clave de correo del colaborador</param>
        /// <param name="correo">Cuenta de correo del colaborador</param>
        /// <returns>Estructura de retorno valor true o false para cuenta office365/returns>
        [Code.TokenAuthorize]
        [Route("api/Colaborador/GetAuthOffice/{clave}/{correo}")]
        // GET api/<controller>/GetAuthOffice/5/prueba@prueba.com
        public bool GetAuthOffice(string clave, string correo)
        {
            return Models.ColaboradorService.CambioCredecialesOffice(correo, clave);
        }

        /// <summary>
        /// Acción que permite renovar el token de sesión de un usuario.
        /// </summary>
        /// <returns>Estructura de datos con la información básica del colaborador autenticado.</returns>
        [Code.TokenAuthorize]
        [Route("api/Colaborador/{uuid}")]
        public Models.Colaborador Put(string uuid)
        {
            string correo = User.Identity.Name;

            // Obtenemos los datos del usuario
            Models.Colaborador usuario = Models.ColaboradorService.Obtiene(correo);

            //Generamos un token para este usuario
            string token = Models.SessionService.GeneraToken(correo, uuid);
            usuario.token = token;
            usuario.expira = DateTime.UtcNow.AddMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["Seguridad.Expiracion"]));

            // Obtenemos los talonarios del usuario
            usuario.talonarios = Models.TalonarioService.ObtienePorUsuario(correo);

            return usuario;
        }
    }
}