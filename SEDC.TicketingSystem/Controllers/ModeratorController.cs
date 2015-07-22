﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEDC.TicketingSystem.Models;
using SEDC.TicketingSystem.ViewModels;
using SEDC.TicketingSystem.Models.Enums;
using SEDC.TicketingSystem.Authorization_Filters;
using System.Net.Mail;

namespace SEDC.TicketingSystem.Controllers
{
    
    [Moderator]
    public class ModeratorController : Controller
    {
        private SEDCTicketingSystemContext db = new SEDCTicketingSystemContext();
        // GET: Moderator
        public ActionResult Index()
        {
            return View();
        }

        // Jordan Show a list of All Tickets in the system
        public ActionResult AllTickets()
        {
            var tickets = db.Tickets.Include(t => t.Moderator).Include(t => t.Owner).OrderBy(d => d.Status);
        
            return View(tickets);
        }

        // Ordering Filters
        public PartialViewResult OrderBy(int? x, int? ord) 
        {
            
            var tickets = db.Tickets.Include(t => t.Moderator).Include(t => t.Owner);
            if (x == 1)
            {
                if (ord != 2)
                    tickets = tickets.OrderBy(d => d.Status);
                else
                    tickets = tickets.OrderByDescending(d => d.Status);
            }
            if (x == 2)
            {
                if (ord != 2)
                    tickets = tickets.OrderBy(d => d.OpenDate);
                else
                    tickets = tickets.OrderByDescending(d => d.OpenDate);
            }
            if (x == 3)
            {
                if (ord != 2)
                    tickets = tickets.OrderBy(d => d.Title);
                else
                    tickets = tickets.OrderByDescending(d => d.Title);
            }
            if (x == 4)
            {
                if (ord != 2)
                    tickets = tickets.OrderBy(d => d.WorkHours);
                else
                    tickets = tickets.OrderByDescending(d => d.WorkHours);
            }
            return PartialView(tickets);
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            TicketAndRepliesViewModel ticketAndRepliesViewModel = new TicketAndRepliesViewModel();
            ticketAndRepliesViewModel.Ticket = ticket;
            ticketAndRepliesViewModel.Replies = db.Replies.Where(x => x.TicketID == id).OrderByDescending(x => x.TimeStamp);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticketAndRepliesViewModel);
        }

        public ActionResult ReOpen(int? id)
        {
            db.Tickets.Find(id).Status = TicketStatus.WaitReply;
            db.SaveChanges();

            return RedirectToAction("AllTickets");
        }


        public ActionResult AssignTicket(int? id)
        {
            List<User> moderators = db.Users.Where(x => x.IsAdmin == true).ToList(); // Get all Moderators
            

            return View(moderators);
        }

        [HttpPost]
        public ActionResult AssignTicket(int moderatorId, string message, int? id)
        {

            db.Tickets.Find(id).ModeratorID = moderatorId;  // change the moderator id on the ticket
            Reply AdminMessage = new Reply(); 
            AdminMessage.TicketID = (int)id;
            AdminMessage.ReplyBody = message;
            AdminMessage.UserID = Convert.ToInt32(Session["LogedUserID"]);
            AdminMessage.IsAdminMessage = true;
            AdminMessage.TimeStamp = DateTime.Now;
            db.Replies.Add(AdminMessage);
            db.SaveChanges();
            SendNotificationEmail((int)id);
            return RedirectToAction("AllTickets");
        }

        //Send Email notification on ticket assign
        public void SendNotificationEmail(int ticketId)
        {

            var user = db.Users.Find(db.Tickets.Find(ticketId).ModeratorID);
            //string ticketUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) +
            //                 "/account/verify?ID=" + confirmationGuid;

            var message = new MailMessage("blindcarrots1@gmail.com", user.Email)
            {
                Subject = "A ticket has been Assigned to you.",
                Body = " Hello " + user.Name +
                         Environment.NewLine + Environment.NewLine + " The ticket with ID: "+ticketId +" has been assigned to you." + Environment.NewLine +
                         "to check the ticket please visit this url: http://localhost:50892/Tickets/Details/" + ticketId

            };

            var client = new SmtpClient();
            var credential = new NetworkCredential
            {
                UserName = "blindcarrots1@gmail.com",  // replace with valid value
                Password = "ticketingSystem"  // replace with valid value
            };
            client.Host= "smtp.gmail.com";
            client.Port= 587;
            client.Credentials = credential;
            client.EnableSsl = true;
            client.Send(message);
        }

    }
}