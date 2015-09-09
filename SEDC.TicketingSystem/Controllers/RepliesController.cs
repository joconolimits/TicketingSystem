using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEDC.TicketingSystem.Models;
using SEDC.TicketingSystem.Models.Enums;
using System.Net.Mail;

namespace SEDC.TicketingSystem.Controllers
{
    public class RepliesController : Controller
    {
        private SEDCTicketingSystemContext db = new SEDCTicketingSystemContext();

        
        //Create a new reply in a ticket. 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]  // We need this to be able to send html through the form.
        public ActionResult Create(int id, string replyBody)
        {
            int loggedUserId = Convert.ToInt32(Session["LogedUserID"]); //  get The id fo the current user
            Reply reply = new Reply();
            reply.TimeStamp = DateTime.UtcNow;  
            reply.UserID = loggedUserId;
            reply.TicketID = id;
            reply.ReplyBody = replyBody;
            db.Replies.Add(reply);
            // If the current user is moderator change  the ticket status and set him as moderator of the ticket.
            if ((AccessLevel)(Session["IsAdmin"]) != AccessLevel.Registered)
            {
                db.Tickets.Find(id).Status = TicketStatus.WaitReply;
                db.Tickets.Find(id).ModeratorID = loggedUserId;
                db.SaveChanges();
                SendNotificationEmail((int)id);  // send email notification to the user/moderator when a new reply

                return RedirectToAction("Details", "Moderator", new { id = id });
            }
            // If he is regular user set the status as pending
            else
            {
                db.Tickets.Find(id).Status = TicketStatus.Pending;
                db.SaveChanges();
                SendNotificationEmail((int)id);  // send email notification to the user/moderator when a new reply

                return RedirectToAction("Details", "Tickets", new { id = id });
            }
        }
         
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Send email notification when a reply is posted 
        public void SendNotificationEmail(int ticketId)
        {
            User user = new User();
            // check  if you need to send notification to user or moderator
            if ((AccessLevel)Session["IsAdmin"] != AccessLevel.Registered)
                user = db.Users.Find(db.Tickets.Find(ticketId).OwnerID);
            else
                user = db.Users.Find(db.Tickets.Find(ticketId).ModeratorID);

            var message = new MailMessage("blindcarrots1@gmail.com", user.Email)
            {
                Subject = "You have new reply for the ticket: " + ticketId,
                Body = " Hello " + user.Name +
                         Environment.NewLine + Environment.NewLine + " You have a new reply. " + Environment.NewLine +
                         "to check the reply of your ticket please visit this url: http://localhost:50892/Tickets/Details/" + ticketId
            };
            // call the email client to send the message 
            SEDC.TicketingSystem.Email.EmailClient.Client(message);
        }
    }
}
