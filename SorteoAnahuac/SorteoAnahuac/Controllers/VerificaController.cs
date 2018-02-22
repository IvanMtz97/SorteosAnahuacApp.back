using SorteoAnahuac.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SorteoAnahuac.Controllers
{
    /// <summary>
    /// Controlador que permite desplegar la información de los boletos vendidos para su verificación
    /// </summary>
    public class VerificaController : Controller
    {
        /// <summary>
        /// Vista de un boleto vendido
        /// </summary>
        /// <returns>Despliegue verificación de un boleto</returns>        
        [Route("boleto/{id}")]
        public ActionResult Index(string id)
        {
            Models.Boleto boleto = Models.TalonarioService.VerificarBoleto(id);
            if (boleto == null)
            {
                return RedirectToAction("Invalido");
            }

            ViewBag.CuentaBancaria = null;
            if (boleto.clave_sorteo.HasValue)
            {
                Sorteo sorteo = SorteoService.Obtener(boleto.clave_sorteo.Value);
                if (sorteo != null)
                    ViewBag.CuentaBancaria = sorteo.cuenta_bancaria;
            }

            ViewBag.ReferenciaBancaria = null;
            if (boleto.clave_colaborador.HasValue)
            {
                Colaborador colaborador = ColaboradorService.ObtienePorClave(boleto.clave_colaborador.Value);
                if (colaborador != null)
                    ViewBag.ReferenciaBancaria = colaborador.referencia_bancaria;
            }
            ViewBag.Boleto = boleto;
            ViewBag.QrCode = Convert.ToBase64String(Models.TalonarioService.GenerateQRCode(string.Format("{0}/boleto/{1}", ConfigurationManager.AppSettings["App.Url.Base"], boleto.token)).ToArray());
            return View();
        }

        /// <summary>
        /// Vista de un boleto vendido
        /// </summary>
        /// <returns>Despliegue verificación de un boleto</returns>        
        [Route("boleto/{id}/folio/{folio_digital}")]
        public ActionResult Detalles(string id, string folio_digital)
        {
            Models.Boleto boleto = Models.TalonarioService.VerificarBoleto(id);
            if (boleto == null || string.IsNullOrEmpty(folio_digital))
            {
                return RedirectToAction("Invalido");
            }

            if (string.IsNullOrEmpty(boleto.folio_digital) || boleto.folio_digital.ToLower() != folio_digital.ToLower())
            {
                return RedirectToAction("Invalido");
            }

            ViewBag.CuentaBancaria = null;
            if (boleto.clave_sorteo.HasValue)
            {
                Sorteo sorteo = SorteoService.Obtener(boleto.clave_sorteo.Value);
                if (sorteo != null)
                    ViewBag.CuentaBancaria = sorteo.cuenta_bancaria;
            }

            ViewBag.ReferenciaBancaria = null;
            if (boleto.clave_colaborador.HasValue)
            {
                Colaborador colaborador = ColaboradorService.ObtienePorClave(boleto.clave_colaborador.Value);
                if (colaborador != null)
                    ViewBag.ReferenciaBancaria = colaborador.referencia_bancaria;
            }
            ViewBag.Boleto = boleto;
            ViewBag.QrCode = Convert.ToBase64String(Models.TalonarioService.GenerateQRCode(string.Format("{0}/boleto/{1}", ConfigurationManager.AppSettings["App.Url.Base"], boleto.token)).ToArray());


            System.Drawing.Image frenteBoleto = Bitmap.FromFile(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Imagenes/Boleto_final_Sorteo_2017.png"));
            Graphics g = Graphics.FromImage(frenteBoleto);
            g.DrawString(boleto.folio, System.Drawing.SystemFonts.DefaultFont, System.Drawing.Brushes.Black, new RectangleF(832, 13, 85, 29));

            MemoryStream frenteImage = new MemoryStream();
            frenteBoleto.Save(frenteImage, System.Drawing.Imaging.ImageFormat.Jpeg);
            frenteImage.Position = 0;

            ViewBag.BoletoFrente = Convert.ToBase64String(frenteImage.ToArray());
            return View();
        }

        /// <summary>
        /// Accion usada para indicar que el token de boleto no es válido
        /// </summary>
        /// <returns>Mensaje de error cuando un token de boleto no es válido</returns>
        public ActionResult Invalido()
        {
            return View();
        }
    }
}