using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SEDC.TicketingSystem.Authorization_Filters
{
    public class IsAdmin : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            filterContext.Controller.ViewBag.AutherizationMessage = "You need to be an Admin to go here.";
        }        
    }
}
