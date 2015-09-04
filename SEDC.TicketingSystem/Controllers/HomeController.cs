
using SEDC.TicketingSystem.HashingAndSalting;
using SEDC.TicketingSystem.Models;
using SEDC.TicketingSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SEDC.TicketingSystem.Controllers
{
    public class HomeController : Controller
    {

        //
        // GET: /Home/
        public ActionResult Login(string LogoutMessage)
        {
            // If Logged in user try to access the Login page redirect him.
            if (HttpContext.User.Identity.IsAuthenticated){
                if ((AccessLevel)Session["IsAdmin"] != AccessLevel.Registered)
                    return RedirectToAction("Index", "Moderator");
                else 
                     return RedirectToAction("WelcomePage");
            }
                  
            var Message = LogoutMessage;
            if (Message == null)
            {
               Message = "Login to use the System.";
            }
           
                ViewBag.Message = Message;
            
            return View();
        }

      
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Email, string Password)
        {
            // this action is for handle post (login)
            if (ModelState.IsValid) // this is check validity
            {
                using (SEDCTicketingSystemContext dc = new SEDCTicketingSystemContext())
                {
                    var v = dc.Users.Where(a => a.Email.Equals(Email)).FirstOrDefault();
                    PasswordManager pwdManager = new PasswordManager();
                    if (v != null && pwdManager.IsPasswordMatch(Password, v.Salt, v.Password))
                    {
                        FormsAuthentication.SetAuthCookie(v.Username, false);
                 
                        Session["CurrentUser"] = v;
                        Session["Username"] = v.Username.ToString();
                        Session["LogedUserID"] = v.ID.ToString();
                        Session["LogedUserFullname"] = v.Name.ToString();
                        Session["IsAdmin"] = v.IsAdmin;
                        if(v.IsAdmin != AccessLevel.Registered)
                            return RedirectToAction("Index", "Moderator", new { id = v.ID }); 
                        else
                            return RedirectToAction("WelcomePage", "Home", new{id = v.ID});   
                    }
                    else
                    {
                        ViewBag.Message = "OOPS :(  You most likely forgot your email or Password!!!";
                    }

                } // end using
            } //end if
            return View();
        }

        // this action is for Logout.
        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            var Message = Session["LogedUserFullname"] + " you are succesfully logged out.";
            Session.Clear();
            return RedirectToAction("Login", new {LogoutMessage = Message});
            
            
        }

        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(string firstName, string lastName, string email, string message)
        {
            var mailMessage = new MailMessage(email, "blindcarrots1@gmail.com")
            {
                Subject = "Contact Form",
                Body = message + string.Format(" <br/><br/><br/>By: {0} {1}", firstName, lastName)
            };

            SEDC.TicketingSystem.Email.EmailClient.Client(mailMessage);
            ViewBag.Message = "The email was sent successfully!";
            return View();
        }


        public ActionResult WelcomePage()
        {
            return View();
        }
        public ActionResult HowItWorks()
        {
            return View();
        }

	}
}