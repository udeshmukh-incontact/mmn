using Microsoft.IdentityModel.Web;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace ManageMyNotificationsMVC.Controllers
{
    public class NotificationsController : BaseController
    {
        // GET: Notification
        public ActionResult Index(string partner)
        {
            if (string.IsNullOrEmpty(partner))
                return RedirectToAction("Manage");
            else
                return RedirectToAction("Manage", new { id = "pb-"+partner });
        }
        [AllowAnonymous] // This is a temporary workaround. Need to fix.
        public ActionResult logout(bool timeout = false)
        {
            string absoluteUrl = Request.Url.AbsoluteUri;
            string replyUrl;
            if (timeout)
                replyUrl = Url.Action("Index", "SessionTimeOut", new { user = User.GetAdfsGuid() }, Request.Url.Scheme);
            else
                replyUrl = absoluteUrl.Substring(0, absoluteUrl.LastIndexOf("/") + 1);
            WSFederationAuthenticationModule.FederatedSignOut(new Uri(FederatedAuthentication.WSFederationAuthenticationModule.Issuer), new Uri(replyUrl));
            return null;
        }

        public ActionResult Manage()
        {
            var configvalue = ConfigurationManager.AppSettings["MaintenanceHideForResellChildAccounts"].ToString();
            ViewBag.configBU = configvalue;
            ViewBag.partner = User.GetAdfsGuid(); 
            return View();
        }
    }

    }