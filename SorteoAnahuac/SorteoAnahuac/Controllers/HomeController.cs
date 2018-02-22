using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SorteoAnahuac.Controllers
{
    /// <summary>
    /// Controlador donde viven la página de inicio, login y logout para usuarios administrativos.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Acción que pinta la pantalla de inicio
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Sorteo Anáhuac - Verificación de Boletos en Línea";

            return View();
        }

        /// <summary>
        /// Acción que permite loggear a un usuario
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            string correo = Request.Params["usuario"];
            string password = Request.Params["password"];
            bool error = false;

            if (String.IsNullOrEmpty(correo) || String.IsNullOrEmpty(password))
            {
                error = true;
                TempData["error"] = "El usuario y contraseña son requeridos.";
            }
            Models.Usuario usuario = null;
            if (!error) {
                usuario = Models.UsuarioService.Auntenticar(correo, password);
                if (usuario == null)
                {
                    error = true;
                    TempData["error"] = "El usuario o contraseña son invalidos.";
                }
            }

            if (error) { 
                return RedirectToAction("Index");
            }

            FormsAuthentication.SetAuthCookie(usuario.identificador + "hola", false);
            return RedirectToAction("Index", "Notificaciones");
        }

        /// <summary>
        /// Acción que permite cerrar la sesión de usuario
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Permite realizar la autenticación de un colaborador, obtener sus datos y un token de sesión
        /// </summary>
        /// <returns>Estructura de datos con la información básica del colaborador autenticado.</returns>
        // GET api/<controller>
        //public JsonResult Colaborador()
        //{

        //    bool autorizado = false;
        //    string correo = string.Empty;
        //    string authHeader = Request.Headers.Get("authorization");
        //    if (authHeader != null)
        //    {
        //        authHeader = authHeader.Split(',')[0];
        //        string headerValor = authHeader.Split(' ')[1];
        //        string headerScheme = authHeader.Split(' ')[0];
        //        if (!String.IsNullOrEmpty(headerValor) && headerScheme == "Basic")
        //        {
        //            string tokenAuth = headerValor.Replace("Basic ", "");
        //            byte[] data = Convert.FromBase64String(tokenAuth);
        //            string decodedString = Encoding.UTF8.GetString(data);
        //            string usuarioAuth = decodedString.Split(':')[0];
        //            string password = decodedString.Split(':')[1];
        //            string ip_address = Request.UserHostAddress;
        //            autorizado = Models.ColaboradorService.Auntenticar(usuarioAuth, password, ip_address);

        //            if (autorizado)
        //            {
        //                correo = usuarioAuth;
        //            }

        //        };
        //    }

        //    if (autorizado) { 

        //        // Obtenemos los datos del usuario
        //        Models.Colaborador usuario = await Models.ColaboradorService.ObtieneConTalonarios(correo);

        //        //Generamos un token para este usuario
        //        string token = Models.SessionService.GeneraToken(correo);
        //        usuario.token = token;
        //        usuario.expira = DateTime.UtcNow.AddMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["Seguridad.Expiracion"]));

        //        return Json(usuario, JsonRequestBehavior.AllowGet);
        //    } else
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //        Response.SuppressFormsAuthenticationRedirect = true;
        //        return Json(new { Error = "Acceso No Autorizado" }, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}
