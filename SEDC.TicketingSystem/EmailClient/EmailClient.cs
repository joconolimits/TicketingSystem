using SEDC.TicketingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SEDC.TicketingSystem.Email
{
    public static class EmailClient
    {
        public static void Client(MailMessage message)
        {

            var client = new SmtpClient();
            var credential = new NetworkCredential
            {
                UserName = "blindcarrots1@gmail.com",  // replace with valid value
                Password = "ticketingSystem"  // replace with valid value
            };
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.Credentials = credential;
            client.EnableSsl = true;
            client.Send(message);
        }
    }
}
