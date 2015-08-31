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
            // If there is no loged in user Redirect him to login
            if (user == null)
            {
                RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                redirectTargetDictionary.Add("action", "Login");
                redirectTargetDictionary.Add("controller", "Home");
                filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
                this.OnActionExecuting(filterContext);
            }
            //check if user is SuperAdmin  Allow the action
            else
            {
                if (user.IsAdmin == AccessLevel.SuperAdmin)
                {
                    this.OnActionExecuting(filterContext);
                }
                // If the user is Moderator Redirect him to Moderator/index page.
                if (user.IsAdmin == AccessLevel.Moderator)
                {
                    RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                    redirectTargetDictionary.Add("action", "Index");
                    redirectTargetDictionary.Add("controller", "Moderator");
                    filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
                    this.OnActionExecuting(filterContext);
                }
                if (user.IsAdmin == AccessLevel.Registered)
                {
                    // if it is just a registered user redirect him to home 
                    RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                    redirectTargetDictionary.Add("action", "WelcomePage");
                    redirectTargetDictionary.Add("controller", "Home");
                    filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
                    this.OnActionExecuting(filterContext);
                }
            }
                
        }
    }
}
