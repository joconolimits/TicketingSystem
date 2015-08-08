using SEDC.TicketingSystem.Authorizatin_Filters;
using SEDC.TicketingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SEDC.TicketingSystem.Controllers
{
    [SuperAdmin]
    public class SuperAdminController : Controller
    {
        private SEDCTicketingSystemContext db = new SEDCTicketingSystemContext();

        // GET: SuperAdmin
        public ActionResult Users()
        {

            return View(db.Users.ToList());
        }
    }
}