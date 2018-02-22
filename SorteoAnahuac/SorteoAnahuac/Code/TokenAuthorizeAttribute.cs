using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace SorteoAnahuac.Code
{
    public class TokenAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {

            bool autorizado = false;
            string authHeader = actionContext.Request.Headers.Where(h => h.Key.ToLower() == "authorization").Select(h => h.Value.FirstOrDefault()).FirstOrDefault();
            if (authHeader != null)
            {
                authHeader = authHeader.Split(',')[0];
                string headerValor = authHeader.Split(' ')[1];
                string headerScheme = authHeader.Split(' ')[0];
                if (!String.IsNullOrEmpty(headerValor) && headerScheme == "Bearer")
                {
                    string token = headerValor.Replace("Bearer ", "");
                    autorizado = Models.SessionService.ValidaToken(token);

                    if (autorizado)
                    {
                        string usuario = Models.SessionService.DescifraToken(token).correo;
                        if (!String.IsNullOrEmpty(usuario))
                        {
                            Models.Colaborador infoUsuario = Models.ColaboradorService.Obtiene(usuario);
                            if (infoUsuario != null)
                            {
                                actionContext.ControllerContext.RequestContext.Principal = new GenericPrincipal(new GenericIdentity(usuario), null);
                            }
                        };
                    }

                };
            }

            return autorizado;
        }

    }
}