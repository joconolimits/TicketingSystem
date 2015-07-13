﻿
using SEDC.TicketingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    var v = dc.Users.Where(a => a.Email.Equals(Email) && a.Password.Equals(Password)).FirstOrDefault();
                    if (v != null)
                    {
                        FormsAuthentication.SetAuthCookie(v.Username, false);
                        // Trying something out with Session 
                        Session["CurrentUser"] = v;
                        Session["Username"] = v.Username.ToString();
                        Session["LogedUserID"] = v.ID.ToString();
                        Session["LogedUserFullname"] = v.Name.ToString();
                        Session["IsAdmin"] = v.IsAdmin;
                        if (v.IsAdmin)
                        {
                            string Username = v.Username;
                            string RoleName = "Admin";
                           // Roles.AddUserToRole(Username, RoleName);
                        }

                        return RedirectToAction("Index", "Tickets", new{id = v.ID});
                       
                    }
                    else
                    {
                        ViewBag.Message = "OOPS :(  You most likely forgot your email or Password!!!";
                    }

                }
            }
            return View();
        }

        // this action is for Logout.
        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            var Message = Session["Username"]+ " you are succesfully logged out.";
            Session.Clear();
            return RedirectToAction("Login", new {LogoutMessage = Message});
            
            
        }

        //[Authorize]
        
        //public ActionResult MyTickets()
        //{
        //    if (Session["Username"] != null)
        //    {

        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login");
        //    }
        //}
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }
        public ActionResult WelcomePage()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
	}
}