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
using SEDC.TicketingSystem.Authorizatin_Filters;
using SEDC.TicketingSystem.HashingAndSalting;
using System.Net.Mail;

namespace SEDC.TicketingSystem.Controllers
{
    
    public class UsersController : Controller
    {
        private SEDCTicketingSystemContext db = new SEDCTicketingSystemContext();

        // GET: Users
        // Only the superAdmin can see the list of users
        [SuperAdmin]
        public ActionResult Index()
        {
            
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
            {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            // If Logged in user try to access the Register page redirect him.
            if (HttpContext.User.Identity.IsAuthenticated && (AccessLevel)Session["IsAdmin"] != AccessLevel.Registered)
                return RedirectToAction("Index", "Moderator");
            else
                if (HttpContext.User.Identity.IsAuthenticated)
                    return RedirectToAction("WelcomePage", "Home");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,LastName,Username,Email,Password, IsAdmin")] User user)
        {
            if (ModelState.IsValid)
            {
                PasswordManager pwdManager = new PasswordManager();

                user.Salt = SaltGenerator.GetSaltString();
                user.Password = pwdManager.GeneratePasswordHash(user.Password, user.Salt);
                db.Users.Add(user);
                db.SaveChanges();
                if (HttpContext.User.Identity.IsAuthenticated && (AccessLevel)Session["IsAdmin"] == AccessLevel.SuperAdmin)
                    return RedirectToAction("Index");
                else
                {
                    return RedirectToAction("Login", "Home");
                }
                   
            }

            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,LastName,Username,Email,Password,IsAdmin")] User user)
        {
            if (ModelState.IsValid)
            {
                PasswordManager pwdManager = new PasswordManager();
                user.Salt = SaltGenerator.GetSaltString();
                user.Password = pwdManager.GeneratePasswordHash(user.Password, user.Salt);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new{id = user.ID });
            }
            return View(user);
        }

        // GET: Users/Delete/5
        //Onlly the SuperAdmin can delete Users from the system.
        [SuperAdmin]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SuperAdmin]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            // If the user that we delete is moderator then remove him from the categories table.
            if (user.IsAdmin != AccessLevel.Registered)
            {
                foreach (var item in db.Categories.Where(t => t.ModeratorID == id))
                {
                    // set the first super admin to be moderator on those categories
                    item.ModeratorID = db.Users.Where(t => t.IsAdmin == AccessLevel.SuperAdmin).FirstOrDefault().ID;
                }

                foreach (var item in db.Tickets.Where(t => t.ModeratorID == id))
                {
                    // set the first super admin to be moderator on those Tickets
                    item.ModeratorID = db.Users.Where(t => t.IsAdmin == AccessLevel.SuperAdmin).FirstOrDefault().ID;
                }
                
                
            }
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email)
        {
            var user = db.Users.Where(t => t.Email == email).FirstOrDefault();
            if (user !=null)
            {
                var guid = Guid.NewGuid();
                user.Guid = guid;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                // Send the email
                var message = new MailMessage("blindcarrots1@gmail.com", user.Email)
                {
                    Subject = "Reset your Password",
                    Body = "Hello " + user.Name + Environment.NewLine + Environment.NewLine + "Click on the bellow link to reset your password: " + Environment.NewLine +
                         "http://localhost:50892/Users/ResetPassword?guid=" + user.Guid
                };
                // call the email client to send the message 
                SEDC.TicketingSystem.Email.EmailClient.Client(message);
                var Message = "An email has been sent to you with further instructions" + Environment.NewLine + "Please check your mail box.";
                return RedirectToAction("Login", "Home", new { LogoutMessage = Message });
            }
            else
            {
                var Message = "The Email: " + email + " is is not regsitered in our system." + Environment.NewLine + "Please  register to use the system";
                return RedirectToAction("Login", "Home", new { LogoutMessage = Message });
            }
        }

        public ActionResult ResetPassword(Guid guid)
        {
            if (guid != Guid.Empty)
            {
                 var user = db.Users.Where(t => t.Guid == guid).FirstOrDefault();
                 return View(user);
            }
            else
                return RedirectToAction("Login", "Home"); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(Guid guid, string Password)
        {
            var user = db.Users.Where(t => t.Guid == guid).FirstOrDefault();
            if (user != null)
            { 
                PasswordManager pwdManager = new PasswordManager();
                user.Salt = SaltGenerator.GetSaltString();
                user.Password = pwdManager.GeneratePasswordHash(Password, user.Salt);
                user.Guid = Guid.Empty;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Login", "Home");
            }
            else
            {
                var Message = "The URL you've used is not valid URL for password reset"+ Environment.NewLine + "Please use a valid URL.";
                return RedirectToAction("Login", "Home", new { LogoutMessage = Message });
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
    }
}
