using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace SorteoAnahuac.Code
{
    /// <summary>
    /// Atributo que permite el validar una sesión de usuario usando el mecánismo de basic authorize
    /// </summary>
    public class BasicAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Función auxiliar para obtener la dirección IP del usuario que se coencta.
        /// </summary>
        /// <param name="request">Objeto Request del cual se quiere extraer la dirección IP</param>
        /// <returns>Direccíon IP del request</returns>
        public string GetClientIpAddress(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return IPAddress.Parse(((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress).ToString();
            }
            return null;
        }

        /// <summary>
        /// Método que se dispara cuando el usuario intenta acceder a la acción protegida por este atributo. Válida los datos de acceso de sesión.
        /// </summary>
        /// <param name="actionContext">Contexto de la petición del usuario</param>
        /// <returns>Booleano indicando si la sesión es válida o no.</returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {

            bool autorizado = false;
            string authHeader = actionContext.Request.Headers.Where(h => h.Key.ToLower() == "authorization").Select(h => h.Value.FirstOrDefault()).FirstOrDefault();
            if (authHeader != null)
            {
                authHeader = authHeader.Split(',')[0];
                string[] partesHeader = authHeader.Split(' ');
                string headerValor = partesHeader[1];
                string headerScheme = partesHeader[0];
                if (!String.IsNullOrEmpty(headerValor) && headerScheme == "Basic")
                {
                    string token = headerValor.Replace("Basic ", "");
                    byte[] data = Convert.FromBase64String(token);
                    string decodedString = Encoding.UTF8.GetString(data);
                    string[] partes = decodedString.Split(':');
                    string usuario = partes[0];
                    string password = partes[1];
                    string ip_address = GetClientIpAddress(actionContext.Request);
                    autorizado = Models.ColaboradorService.Auntenticar(usuario, password, ip_address);

                    if (autorizado)
                    {
                        actionContext.ControllerContext.RequestContext.Principal = new GenericPrincipal(new GenericIdentity(usuario), null);
                    }

                };
            }

            return autorizado;
        }

    }
}