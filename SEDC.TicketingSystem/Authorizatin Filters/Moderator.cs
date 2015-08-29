using SEDC.TicketingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using SEDC.TicketingSystem.Models.Enums;

namespace SEDC.TicketingSystem.Authorization_Filters
{
    public class Moderator : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {

            //get user from session["CurrentUser"]
            var user = (User)System.Web.HttpContext.Current.Session["CurrentUser"];

            if (user == null)
            {
                RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                redirectTargetDictionary.Add("action", "Login");
                redirectTargetDictionary.Add("controller", "Home");
                filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
                this.OnActionExecuting(filterContext);
            }
            //check if user is Moderator or super user 
            else 
                if (user.IsAdmin == AccessLevel.Moderator || user.IsAdmin == AccessLevel.SuperAdmin)
                {
                    //if yes continue
                    this.OnActionExecuting(filterContext);
                }
                else
                {
                    // if not redirect to home 
                    RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                    redirectTargetDictionary.Add("action", "WelcomePage");
                    redirectTargetDictionary.Add("controller", "Home");
                    filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
                    this.OnActionExecuting(filterContext);
                }
            
        }

    }
}
