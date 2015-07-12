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
            return View(db.Tickets.ToList().OrderBy(x => x.Status));
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

    }
}