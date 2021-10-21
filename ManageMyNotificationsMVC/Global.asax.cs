using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ManageMyNotificationsMVC
{
    [ExcludeFromCodeCoverage]
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            /*
             * This is a hack to set the tls version because salesforce doesn't allow TLS 1.0
             * We will try and negotiate with whichever version is the latest. Should come back and evaluate this
             */
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            AreaRegistration.RegisterAllAreas();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        //protected void Application_Error()
        //{
        //    Exception ex = Server.GetLastError();
        //    Exception innerEx = ex.InnerException;
        //    //Log the user out if an error occurs related to their authentication cookie
        //    if (ex.GetType() == typeof(CryptographicException) || (innerEx != null && innerEx.GetType() == typeof(CryptographicException)))
        //    {
        //        Uri signOutUrl = new Uri(FederatedAuthentication.WSFederationAuthenticationModule.Issuer);
        //        WSFederationAuthenticationModule.FederatedSignOut(signOutUrl, Request.Url);
        //    }
        //}
    }
}
