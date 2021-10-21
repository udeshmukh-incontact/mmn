using Elmah;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Routing;

namespace ManageMyNotificationsMVC.Common
{
    [ExcludeFromCodeCoverage]
    public class UnhandledErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                ErrorSignal.FromCurrentContext().Raise(filterContext.Exception);
                filterContext.ExceptionHandled = true;
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    //Redirect to the error page if an unraised exception occurs
                    var routeDictionary = new RouteValueDictionary { { "area", null }, { "controller", "Error" }, { "action", "Index" } };
                    filterContext.Result = new RedirectToRouteResult(routeDictionary);
                }
            }
        }
    }
}