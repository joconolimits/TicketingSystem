﻿using System;
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

        // GET: Replies
        // Jordan I think  we don't need it
        //public ActionResult Index()
        //{
        //    var replies = db.Replies.Include(r => r.Ticket).Include(r => r.User);
        //    return View(replies.ToList());
        //}

        // GET: Replies/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Reply reply = db.Replies.Find(id);
        //    if (reply == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(reply);
        //}

        // GET: Replies/Create
        //public ActionResult Create()
        //{
        //    ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title");
        //    ViewBag.UserID = new SelectList(db.Users, "ID", "Name");
        //    return View();
        //}

        // POST: Replies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        // Jordan Custom Action to Create a new reply in a ticket. 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id, string replyBody)
        {
            int loggedUserId = Convert.ToInt32(Session["LogedUserID"]);
            Reply reply = new Reply();
            reply.TimeStamp = DateTime.Now;
            reply.UserID = loggedUserId;
            reply.TicketID = id;
            reply.ReplyBody = replyBody;
            db.Replies.Add(reply);
            if (Convert.ToInt32(Session["IsAdmin"]) == 1)
            {
                db.Tickets.Find(id).Status = TicketStatus.WaitReply;
                db.Tickets.Find(id).ModeratorID = loggedUserId;
            }
            else
            {
                db.Tickets.Find(id).Status = TicketStatus.Pending;
            }
            db.SaveChanges();
            SendNotificationEmail((int)id);  // send email notification to the user / Admin when a new reply
        
            return RedirectToAction("Details", "Tickets", new { id = id });
            //if (ModelState.IsValid)
            //{
            //    db.Replies.Add(reply);
            //    db.SaveChanges();
                
            //}

            //ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", reply.TicketID);
            //ViewBag.UserID = new SelectList(db.Users, "ID", "Name", reply.UserID);
            //return View(reply);
        }

        // GET: Replies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reply reply = db.Replies.Find(id);
            if (reply == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", reply.TicketID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Name", reply.UserID);
            return View(reply);
        }

        // POST: Replies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TicketID,ReplyBody,UserID,TimeStamp")] Reply reply)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reply).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "Title", reply.TicketID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Name", reply.UserID);
            return View(reply);
        }

        // GET: Replies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reply reply = db.Replies.Find(id);
            if (reply == null)
            {
                return HttpNotFound();
            }
            return View(reply);
        }

        // POST: Replies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reply reply = db.Replies.Find(id);
            db.Replies.Remove(reply);
            db.SaveChanges();
            return RedirectToAction("Index");
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
            if (Convert.ToInt32(Session["IsAdmin"]) == 1)
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
