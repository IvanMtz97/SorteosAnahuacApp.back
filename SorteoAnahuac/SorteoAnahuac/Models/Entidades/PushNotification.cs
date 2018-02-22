using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SorteoAnahuac.Models
{
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