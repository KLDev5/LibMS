using System;
using System.Web;
using System.Web.Mvc;


namespace LibraryManagement.Filters
{
    public class SessionAuthFilters: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;

            // Allow login and register actions to be accessed without authentication
            if (controller == "Login" && (action == "Login" || action == "Register" || action == "Logout"))
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            
            if (session["UserId"] == null)
            {
                
                // Redirect to login page if session is missing
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Login" },
                        { "action", "Login" }
                    });

                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}