using SEDC.TicketingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEDC.TicketingSystem.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Login()
        {
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
                        Session["LogedUserID"] = v.Username.ToString();
                        Session["LogedUserFullname"] = v.Name.ToString();
                        return RedirectToAction("MyTickets");
                    }
                }
            }
            return View();
        }

        public ActionResult MyTickets()
        {
            if (Session["LogedUserID"] != null)
            {

                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult Register() 
        {
            return View();
        }
	}
}