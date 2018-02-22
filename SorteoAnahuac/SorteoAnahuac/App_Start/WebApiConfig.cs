using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SorteoAnahuac
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Permitimos peticiones de CORS
            config.EnableCors();

            // Cambiamos el formateador de Json a JIL para mayor performance
            config.Formatters.RemoveAt(0);
            config.Formatters.Insert(0, new Code.JilFormatter());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
