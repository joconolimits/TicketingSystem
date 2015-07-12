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
            var tickets = db.Tickets.Where(x => x.OwnerID == id);
           // var tickets = db.Tickets.Include(t => t.Moderator).Include(t => t.Owner);
            return View(tickets.ToList());
        }

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
            ticketAndRepliesViewModel.Replies = db.Replies.Where(x => x.TicketID == id).OrderByDescending(x => x.TimeStamp);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticketAndRepliesViewModel);
        }

        // Jordan Method for the user to close his ticket
        public ActionResult Close(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            db.Tickets.Find(id).Status = TicketStatus.Closed;
            db.SaveChanges();
            return RedirectToAction("index", new { id = Convert.ToInt32(Session["LogedUserID"]) });
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            //ViewBag.ModeratorID = new SelectList(db.Users, "ID", "Name");
            //ViewBag.OwnerID = new SelectList(db.Users, "ID", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Body")] Ticket ticket)
        {
           
            if (ModelState.IsValid)
            {
                ticket.OwnerID = Convert.ToInt32(Session["LogedUserID"]); // Jordan Set The owner Id to the id of the Current user.
                ticket.ModeratorID = 1; // Jordan All tickets are assigned to one Moderator. He will reasign them to others.
                ticket.OpenDate = DateTime.Now;
                ticket.CloseDate = DateTime.MaxValue;
                ticket.Status = TicketStatus.Pending;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index", new {id = ticket.OwnerID });
            }

            ViewBag.ModeratorID = new SelectList(db.Users, "ID", "Name", ticket.ModeratorID);
            ViewBag.OwnerID = new SelectList(db.Users, "ID", "Name", ticket.OwnerID);
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
            ViewBag.ModeratorID = new SelectList(db.Users, "ID", "Name", ticket.ModeratorID);
            ViewBag.OwnerID = new SelectList(db.Users, "ID", "Name", ticket.OwnerID);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Body,Status,OwnerID,ModeratorID,OpenDate,CloseDate,WorkHours")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new {id = ticket.OwnerID });  /*Here I return The ownerID as well So it will show only  the Tickets raised by the loged in user */
            }
            ViewBag.ModeratorID = new SelectList(db.Users, "ID", "Name", ticket.ModeratorID);
            ViewBag.OwnerID = new SelectList(db.Users, "ID", "Name", ticket.OwnerID);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
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
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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
    }
}
