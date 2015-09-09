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
            message.IsBodyHtml = true;
            var client = new SmtpClient();
            var credential = new NetworkCredential
            {
                UserName = "blindcarrots1@gmail.com",  
                Password = "ticketingSystem"  
            };
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.Credentials = credential;
            client.EnableSsl = true;
            client.Send(message);
        }
    }
}
