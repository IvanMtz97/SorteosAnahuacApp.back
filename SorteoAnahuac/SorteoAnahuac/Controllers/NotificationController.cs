using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SorteoAnahuac.Controllers
{
    public class NotificationController : ApiController
    {
        [HttpPost]
        [Route("api/Notification")]
        public HttpResponseMessage PostNotification([FromBody]Dictionary<string, string> pushNotification)
        {


            return Request.CreateResponse(HttpStatusCode.OK, pushNotification);
        }

        [HttpGet]
        [Route("api/Notification/{texto}")]
        public IHttpActionResult Get(string texto)
        {
            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create("https://api.everlive.com/v1/rhcj0gm2mr89cdtx/Push/Notifications");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            PushNotification push = new PushNotification();
            push.Message = texto;
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

            return Ok(responseFromServer);
        }
    }

    public class PushNotification
    {
        private string message = String.Empty;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
