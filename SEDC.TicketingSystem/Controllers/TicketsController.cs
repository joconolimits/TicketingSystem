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
using System.Net.Mail;

namespace SEDC.TicketingSystem.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private SEDCTicketingSystemContext db = new SEDCTicketingSystemContext();

        // GET: Tickets
        // Show My tickets page.
        public ActionResult Index(int? id)
        {
            //var tickets = db.Tickets.Where(x => x.OwnerID == id);
             var tickets = db.Tickets.Include(t => t.Moderator).Include(t => t.Owner).Include(t => t.Category).Where(x => x.OwnerID == id)
                 .OrderBy(t => t.Status).ThenBy(t => t.OpenDate);

            // Add the categories picklist into the view it is needed for teh filter
             ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            return View(tickets.ToList());
        }

        // Filter By Method
        public PartialViewResult FilterBy(int? categoryId, int? statusId)
        {
            var id = Convert.ToInt32(Session["LogedUserID"]);
            var tickets = db.Tickets.Include(t => t.Moderator).Include(t => t.Owner).Include(t => t.Category).Where(t => t.OwnerID == id);

            if(categoryId != null)
                tickets = tickets.Where(t => t.CategoryID == categoryId);
            if(statusId != null)
                tickets = tickets.Where(t => t.Status == (TicketStatus)statusId);
            return PartialView(tickets.OrderBy(t => t.Status).ToList());
        }

        // Ordering Filters
        //public PartialViewResult OrderBy(int? x, int? ord)
        //{
        //    var id = Convert.ToInt32(Session["LogedUserID"]);
        //    var tickets = db.Tickets.Include(t => t.Moderator).Include(t => t.Owner).Include(t => t.Category).Where(t => t.OwnerID == id);
        //    if (x == 1)
        //    {
        //        if (ord != 2)
        //            tickets = tickets.OrderBy(d => d.Status);
        //        else
        //            tickets = tickets.OrderByDescending(d => d.Status);
        //    }
        //    if (x == 2)
        //    {
        //        if (ord != 2)
        //            tickets = tickets.OrderBy(d => d.OpenDate);
        //        else
        //            tickets = tickets.OrderByDescending(d => d.OpenDate);
        //    }
        //    if (x == 3)
        //    {
        //        if (ord != 2)
        //            tickets = tickets.OrderBy(d => d.Title);
        //        else
        //            tickets = tickets.OrderByDescending(d => d.Title);
        //    }
        //    if (x == 4)
        //    {
        //        if (ord != 2)
        //            tickets = tickets.OrderBy(d => d.WorkHours);
        //        else
        //            tickets = tickets.OrderByDescending(d => d.WorkHours);
        //    }
        //    return PartialView(tickets.ToList());
        //}


        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            TicketAndRepliesViewModel ticketAndRepliesViewModel = new TicketAndRepliesViewModel();
            ticketAndRepliesViewModel.Ticket = ticket;
            ticketAndRepliesViewModel.Replies = db.Replies.Where(x => x.TicketID == id && x.IsAdminMessage == false).OrderByDescending(x => x.TimeStamp);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticketAndRepliesViewModel);
        }

        // Jordan Method for the user to close his ticket for both admin and user
        public ActionResult Close(int? id, int? workHours)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            db.Tickets.Find(id).Status = TicketStatus.Closed;
            db.Tickets.Find(id).CloseDate = DateTime.UtcNow;
             if (workHours == null)  // If the user closed the ticket the work hours will be the  diference between the closed and open time.
            {
                workHours = (int)(db.Tickets.Find(id).CloseDate - db.Tickets.Find(id).OpenDate).TotalHours;
            } 
            db.Tickets.Find(id).WorkHours = (int)workHours;
            db.SaveChanges();
            if ((AccessLevel)Session["IsAdmin"] != AccessLevel.Registered)
            {
                return RedirectToAction("AllTickets", "Moderator", new { id = Convert.ToInt32(Session["LogedUserID"]) });
            }
            return RedirectToAction("index", new { id = Convert.ToInt32(Session["LogedUserID"]) });
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Body, CategoryID")] Ticket ticket)
        {
           
            if (ModelState.IsValid)
            {
                ticket.OwnerID = Convert.ToInt32(Session["LogedUserID"]); // Jordan Set The owner Id to the id of the Current user.
                // set moderatorID  to the id of the moderator who is assigned to that category.
                ticket.ModeratorID = db.Categories.FirstOrDefault(t => t.ID == ticket.CategoryID).ModeratorID; 
                ticket.OpenDate = DateTime.UtcNow;
                ticket.CloseDate = DateTime.MaxValue;
                ticket.Status = TicketStatus.Pending;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                SendNotificationEmail(ticket.ID);
                return RedirectToAction("Index", new {id = ticket.OwnerID });
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // jordan removed  fields from the bind, same  needs to be removed from  the view
        public ActionResult Edit([Bind(Include = "ID,Title,Body, CategoryID ")] Ticket ticket)
        {
            var orgTicket = db.Tickets.Find(ticket.ID);
            if (ModelState.IsValid)
            {
                orgTicket.Title = ticket.Title;
                orgTicket.Body = ticket.Body;
                orgTicket.CategoryID = ticket.CategoryID;

                db.Entry(orgTicket).State = EntityState.Modified;
                db.SaveChanges();
                if((AccessLevel)Session["IsAdmin"] != AccessLevel.Registered)
                    return RedirectToAction("AllTickets", "Moderator");
                else
                    return RedirectToAction("Index", new {id = orgTicket.OwnerID });  /*Here I return The ownerID as well So it will show only  the Tickets raised by the loged in user */
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            return View(orgTicket);
        }

         //GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Include(t => t.Moderator).Include(t => t.Owner).Where(t => t.ID == id).FirstOrDefault();
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            // Delete all replies that are assosiated with the ticket.
            var replies = db.Replies.Where(t => t.TicketID == id).ToArray();
            foreach (var item in replies)
            {
                db.Replies.Remove(item);
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { id = ticket.OwnerID });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // Send email notification when a new ticket is created
        public void SendNotificationEmail(int ticketId)
        {
            string categoryName = db.Categories.Find(db.Tickets.Find(ticketId).CategoryID).Name;
            var user = db.Users.Find(db.Tickets.Find(ticketId).ModeratorID);
            var message = new MailMessage("blindcarrots1@gmail.com", user.Email)
            {
                Subject = "New Ticket in category: " + categoryName,
                Body = "Hello " + user.Name + Environment.NewLine + Environment.NewLine + " New ticket with ID: " + ticketId + " has been posted in the category:  "+ categoryName + Environment.NewLine +
                    "to check the ticket please visit this url: http://localhost:50892/Moderator/Details/" + ticketId
            };
            // call the email client to send the message 
            SEDC.TicketingSystem.Email.EmailClient.Client(message);
        }
    }
}
