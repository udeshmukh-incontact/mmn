using ManageMyNotificationsMVC.Common;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ManageMyNotificationsMVC.Controllers
{
    [Authorize]
    [HandleError(ExceptionType = typeof(HttpException))]
    [HandleError(ExceptionType = typeof(NotImplementedException))]
    [HandleError(ExceptionType = typeof(InvalidCastException))]
    public class BaseController : Controller
    {
        protected IElmahWrapper _elmahWrapper { get; set; }

        public BaseController() : this(DependencyResolver.Current.GetService<IElmahWrapper>()) { }

        public BaseController(IElmahWrapper elmahWrapper)
        {
            _elmahWrapper = elmahWrapper;
        }
        [ExcludeFromCodeCoverage]
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            _elmahWrapper.Raise(filterContext.Exception);

            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var routeDictionary = new RouteValueDictionary { { "area", null }, { "controller", "Error" }, { "action", "Index" } };
                filterContext.Result = new RedirectToRouteResult(routeDictionary);
            }
        }

        public bool IsImpersonationModeOn()
        {
            if (User.VerifyImpersonationMode())
                return true;
            else
                return false;
        }
    }
}