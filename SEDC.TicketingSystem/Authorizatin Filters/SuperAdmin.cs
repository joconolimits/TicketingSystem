using SEDC.TicketingSystem.Models;
using SEDC.TicketingSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace SEDC.TicketingSystem.Authorizatin_Filters
{
    public class SuperAdmin : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {

            //get user from session["CurrentUser"]
            var user = (User)System.Web.HttpContext.Current.Session["CurrentUser"];

            //check if user is SuperAdmin or not 
            if (user.IsAdmin != AccessLevel.SuperAdmin)
            {
                // if not redirect to home 
                RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                redirectTargetDictionary.Add("action", "WelcomePage");
                redirectTargetDictionary.Add("controller", "Home");
                filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
                this.OnActionExecuting(filterContext);
            }

            //if yes continue
            this.OnActionExecuting(filterContext);
        }
    }
}
