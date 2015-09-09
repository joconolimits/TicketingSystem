using System;
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

        //Show a list of All Tickets in the system
        public ActionResult AllTickets()
        {
            var tickets = db.Tickets.Include(t => t.Moderator).Include(t => t.Owner).Include(t => t.Category)
                .OrderBy(t => t.Status).ThenBy(t => t.OpenDate);

            // Add the categories picklist into the view it is needed for the filter
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            return View(tickets);
        }

        //Show a list of the New Pending Tickets in the system (Ones who does not have a reply)
        public ActionResult NewPending()
        {
            var tickets = db.Tickets.Include(t => t.Moderator).Include(t => t.Owner).Include(t => t.Replies).Include(t => t.Category);
            tickets = tickets.Where(t =>t.Status == TicketStatus.Pending && t.Replies.Count() == 0).OrderBy(t => t.OpenDate);
            return View(tickets);
        }

        //Show a list of the All Pending Tickets in the system 
        public ActionResult AllPending()
        {
            var tickets = db.Tickets.Include(t => t.Moderator).Include(t => t.Owner).Include(t => t.Category);
            tickets = tickets.Where(t => t.Status == TicketStatus.Pending).OrderBy(d => d.OpenDate);
            return View(tickets);
        }

        // Jordan Show a list of the My Tickets in the system 
        public ActionResult MyTickets()
        {
            var tickets = db.Tickets.Include(t => t.Moderator).Include(t => t.Owner).Include(t => t.Category);
            int ModeratorID = Convert.ToInt32(Session["LogedUserID"]);
            tickets = tickets.Where(t => t.ModeratorID == ModeratorID).OrderBy(t => t.Status).ThenBy(t => t.OpenDate);

            // Add the categories picklist into the view it is needed for teh filter
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            return View(tickets.ToList());
        }

        // Filter By Method
        public PartialViewResult FilterBy(int? categoryId, int? statusId, int? key)
        {
            var tickets = db.Tickets.Include(t => t.Moderator).Include(t => t.Owner).Include(t => t.Category);
            // If true then you need to filter only the tickets of the current moderator
            if (key == 1)
            {
                var id = Convert.ToInt32(Session["LogedUserID"]);
                tickets = tickets.Where(t => t.ModeratorID == id);
            }
            if (categoryId != null)
                tickets = tickets.Where(t => t.CategoryID == categoryId);
            if (statusId != null)
                tickets = tickets.Where(t => t.Status == (TicketStatus)statusId);

            return PartialView(tickets.OrderBy(t => t.Status).ToList());
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

        // Moderator can reopen a ticket that was already closed
        public ActionResult ReOpen(int? id)
        {
            var ticket = db.Tickets.Find(id);
            ticket.Status = TicketStatus.WaitReply;
            ticket.CloseDate = DateTime.MaxValue;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("AllTickets");
        }

        // Assign a ticket to another moderator and send him a message 
        public ActionResult AssignTicket(int? id)
        {
            // Get all Moderators
            List<User> moderators = db.Users.Where(x => x.IsAdmin != AccessLevel.Registered).ToList();
            return View(moderators);
        }

        [HttpPost]
        public ActionResult AssignTicket(int moderatorId, string message, int? id)
        {
            // change the moderator id on the ticket
            db.Tickets.Find(id).ModeratorID = moderatorId;
            //Create the message for the new moderator
            Reply AdminMessage = new Reply(); 
            AdminMessage.TicketID = (int)id;
            AdminMessage.ReplyBody = message;
            AdminMessage.UserID = Convert.ToInt32(Session["LogedUserID"]);
            AdminMessage.IsAdminMessage = true;
            AdminMessage.TimeStamp = DateTime.UtcNow;
            db.Replies.Add(AdminMessage);
            db.SaveChanges();
            //send notification to the new moderator
            SendNotificationEmail((int)id);

            return RedirectToAction("AllTickets");
        }

        // Moderators can search for the ticket they need from the homepage
        public PartialViewResult Search(string query, bool title, bool owner, bool moderator, bool body, bool category)
        {
           IEnumerable<Ticket> searchResults = null;
            // If none of the checklist elements is selected then it will search everywhere
            if (!title && !owner && !moderator && !body && !category) { 
            searchResults = db.Tickets.Include(t => t.Category).Include(t => t.Moderator).Include(t => t.Owner)
                .Where(t =>
                    t.Title.Contains(query) ||
                    t.Body.Contains(query) ||
                    t.Category.Name.Contains(query) ||
                    t.Moderator.Name.Contains(query) ||
                    t.Owner.Name.Contains(query)
                );
            }
            // If any checkbox is selected it will search only in the selected fields
            else {
                searchResults = db.Tickets.Include(t => t.Category).Include(t => t.Moderator).Include(t => t.Owner)
                .Where(t =>
                   ( t.Title.Contains(query) && title == true)||
                   ( t.Body.Contains(query) && body == true) ||
                   ( t.Category.Name.Contains(query) && category == true) ||
                   (t.Moderator.Name.Contains(query) && moderator == true) ||
                   ( t.Owner.Name.Contains(query) && owner == true)
                );
            }
            return PartialView(searchResults);
        }

        //Send Email notification on ticket assign
        public void SendNotificationEmail(int ticketId)
        {
            var user = db.Users.Find(db.Tickets.Find(ticketId).ModeratorID);
            var message = new MailMessage("blindcarrots1@gmail.com", user.Email)
            {
                Subject = "A ticket has been Assigned to you.",
                Body = " Hello " + user.Name +
                         Environment.NewLine + Environment.NewLine + " The ticket with ID: "+ticketId +" has been assigned to you." + Environment.NewLine +
                         "to check the ticket please visit this url: http://localhost:50892/Tickets/Details/" + ticketId
            };

            SEDC.TicketingSystem.Email.EmailClient.Client(message);
        }

    }
}