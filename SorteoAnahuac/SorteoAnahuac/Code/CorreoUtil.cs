using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;

namespace SorteoAnahuac.Code
{
    /// <summary>
    /// Clase que incluye funcionalidades para envío de correos
    /// </summary>
    public static class CorreoUtil
    {
        /// <summary>
        /// Función de envío genérico de correo.
        /// </summary>
        /// <param name="destinatario"></param>
        /// <param name="asunto"></param>
        /// <param name="cuerpo"></param>
        /// <param name="adjuntos"></param>
        public static void Enviar(string[] destinatario, string asunto, string cuerpo, Attachment[] adjuntos = null)
        {

            SmtpSection mailSettings = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            String userName = mailSettings.From;
            String password = mailSettings.Network.Password;
            MailMessage msg = new MailMessage();
            foreach (string destino in destinatario)
            {

                msg.To.Add(new MailAddress(destino));
            }
            msg.From = new MailAddress(userName);
            msg.Subject = asunto;
            //msg.Body = cuerpo;
            msg.IsBodyHtml = true;
            AlternateView body = AlternateView.CreateAlternateViewFromString(cuerpo, null, System.Net.Mime.MediaTypeNames.Text.Html);
            if (adjuntos != null && adjuntos.Length > 0)
            {
                foreach (Attachment archivo in adjuntos)
                {
                    body.LinkedResources.Add(new LinkedResource(archivo.ContentStream, archivo.ContentType)
                    {
                        ContentId = archivo.ContentId,
                        ContentType = archivo.ContentType
                    });
                }
            }
            msg.AlternateViews.Add(body);
            SmtpClient client = new SmtpClient();

            client.Send(msg);
        }
    }
}