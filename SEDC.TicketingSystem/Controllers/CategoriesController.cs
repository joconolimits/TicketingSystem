using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SEDC.TicketingSystem.Models;
using SEDC.TicketingSystem.Authorizatin_Filters;
using SEDC.TicketingSystem.Models.Enums;

namespace SEDC.TicketingSystem.Controllers
{
    [SuperAdmin]
    public class CategoriesController : Controller
    {
        private SEDCTicketingSystemContext db = new SEDCTicketingSystemContext();

        // GET: Categories
        public ActionResult Index()
        {
            var categories = db.Categories.Include(c => c.Moderator);
            return View(categories.ToList());
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            // send all the moderators in the view as select list
            var users = db.Users.Where(t => t.IsAdmin != AccessLevel.Registered);
            ViewBag.ModeratorID = new SelectList(users, "ID", "Name");
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,ModeratorID")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ModeratorID = new SelectList(db.Users, "ID", "Name", category.ModeratorID);
            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            var moderators = db.Users.Where(t => t.IsAdmin != AccessLevel.Registered);
            ViewBag.ModeratorID = new SelectList(moderators, "ID", "Name", category.ModeratorID);
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,ModeratorID")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ModeratorID = new SelectList(db.Users, "ID", "Name", category.ModeratorID);
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id, string message)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.message = message;
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            // Only categories without tickets in them can be deleted
            if (db.Tickets.Count(t => t.CategoryID == id) != 0)
                return RedirectToAction("Delete", new {message = "This category has tickets assigned to it. Make sure there is no tickets in the category before you try to delete it."});
           
            db.Categories.Remove(category);
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
    }
}
