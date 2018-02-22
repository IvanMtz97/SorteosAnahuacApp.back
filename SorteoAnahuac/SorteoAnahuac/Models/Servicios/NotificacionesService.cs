using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SorteoAnahuac.Models
{
    /// <summary>
    /// Servicio que permite enviar notificaciones de texto a los usuarios del app
    /// </summary>
    public static class NotificacionesService
    {
        /// <summary>
        /// Método que envía los menajes
        /// </summary>
        /// <param name="mensaje">Contenido del mensaje a enviar</param>
        public static void Notificar(string mensaje)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create(String.Format("https://api.everlive.com/v1/{0}/Push/Notifications", ConfigurationManager.AppSettings["App.Id"]));
            //Set the authorization header
            request.Headers.Add("Authorization", string.Format("Masterkey {0}", ConfigurationManager.AppSettings["App.MasterKey"]));
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            PushNotification push = new PushNotification();
            push.Message = mensaje;
            string postData = JsonConvert.SerializeObject(push);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/json";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();
        }
    }
}