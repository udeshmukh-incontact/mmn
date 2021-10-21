using InContact.Common.Branding;
using ManageMyNotificationsBusinessLayer.Interfaces;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace ManageMyNotificationsMVC.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IPersonBusinessLogic _PersonLogic;
        public ErrorController(IPersonBusinessLogic personLogic)
        {
            _PersonLogic = personLogic;
        }
        // GET: Error
        public ActionResult Index()
        {
            try
            {
                List<string> accNo;
                try
                {
                    accNo = _PersonLogic.GetTopLevelAccount(User.GetAdfsGuid());
                }
                catch
                {
                    accNo = new List<string>();
                }
                BrandingController bc = new BrandingController();
                string message = bc.GetErrorMessage(accNo.Count > 0 ? string.Join(",", accNo) : "100001");
                ViewBag.ErrorMessage = message;
                return View();
            }
            catch
            {
                string msg = HttpUtility.HtmlDecode(ConfigurationManager.AppSettings["FailoverErrorMessageDetail"].ToString());
                ViewBag.ErrorMessage = msg;
                return View();
            }
        }

        public ActionResult NotAuthorized()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult IncompleteProfile()
        {
            return View();
        }
    }
}