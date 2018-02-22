using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SorteoAnahuac.Controllers
{
    /// <summary>
    /// Controlador que permite el envío de mensajes
    /// </summary>
    [Authorize]
    public class NotificacionesController : Controller
    {
        /// <summary>
        /// Pantalla de envío de mensajes
        /// </summary>
        /// <returns></returns>
        // GET: Notificaciones
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción que permite envíar un mensaje
        /// </summary>
        /// <param name="contenido"></param>
        /// <returns></returns>
        public ActionResult Notificar(string contenido)
        {
            Models.NotificacionesService.Notificar(contenido);
            TempData["resultado"] = "El mensaje ha sido enviado.";
            return RedirectToAction("Index");
        }
    }
}