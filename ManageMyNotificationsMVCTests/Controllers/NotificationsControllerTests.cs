using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsMVC.Controllers;
using ManageMyNotificationsMVC.Models;
using Microsoft.IdentityModel.Claims;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ManageMyNotificationsMVCTests.Controllers
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class NotificationsControllerTests
    {
        private string adfsGuid = "c18a5aa0-d576-447d-a563-44dcba28a509";
        private NotificationsController _controller;

        public NotificationsControllerTests()
        {
            InitController();
        }
        //[Test]
        //public void Logout_TestMethod()
        //{
        //    var fakerequest = new Mock<HttpRequestBase>();
        //    var fakesession = new Mock<HttpSessionStateBase>();
        //    var fakecontext = new Mock<HttpContextBase>();
        //    var fakeIdentity = new GenericIdentity("test@xyzTest.com");
        //    var fakeprincipal = new GenericPrincipal(fakeIdentity, null);
        //    var notificationControllerMock = new Mock<NotificationsController>();
        //    notificationControllerMock.Setup(x => x.HttpContext).Returns(fakecontext.Object);
        //    fakecontext.Setup(t => t.User).Returns(fakeprincipal);
        //    fakerequest.SetupGet(x => x.Headers).Returns(new System.Net.WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });
        //    fakecontext.SetupGet(c => c.Session).Returns(fakesession.Object);
        //    fakecontext.SetupGet(x => x.Request).Returns(fakerequest.Object);
        //    var controller = new NotificationsController();
        //    var result = controller.logout("") as ViewResult;
        //    Assert.AreEqual(null, result.ViewName);
        //}
        [Test]
        public void Index_TestMethod()
        {
            var result = _controller.Index(string.Empty) as RedirectToRouteResult;
            Assert.AreEqual("Manage", result.RouteValues["action"]);
        }

        [Test]
        public void Index_WithPartner_TestMethod()
        {
            var result = _controller.Index("verizon") as RedirectToRouteResult;
            Assert.AreEqual("Manage", result.RouteValues["action"]);
        }

        [Test]
        public void Manage_TestMethod()
        {
            var result = _controller.Manage() as ActionResult;
            Assert.IsNotNull(_controller.ViewBag.configBU);
            Assert.IsNotNull(_controller.ViewBag.partner);
            Assert.IsNotNull(result);
        }

        [Test]
        public void Manage_ViewBag_TestMethod()
        {
            var ar = _controller.Manage() as ViewResult;
            Assert.IsNotNull(ar.ViewData["configBU"]?.ToString());
            Assert.IsNotEmpty(ar.ViewData["configBU"]?.ToString());
            Assert.IsNotNull(ar.ViewData["partner"]?.ToString());
            Assert.IsNotEmpty(ar.ViewData["partner"]?.ToString());
        }

        #region Private Methods
        private void InitController()
        {
            List<Claim> claims = new List<Claim>();
            Claim adfsGuidClaim = new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, Convert.ToBase64String(new Guid(adfsGuid).ToByteArray()));
            claims.Add(adfsGuidClaim);
            Claim userNameClaim = new Claim(System.Security.Claims.ClaimTypes.Name, "TestUserName");
            claims.Add(userNameClaim);

            NotificationManagerViewModel sessionModel = new NotificationManagerViewModel();
            sessionModel.Person = new Persons();
            sessionModel.Person.TimeZone = "TestTZ";
            sessionModel.NotificationGroups = new List<NotificationGroup>();
            sessionModel.Devices = new List<Device>();

            var session = new Mock<HttpSessionStateBase>();
            //session.Object["NotificationManager"] = sessionModel;
            var httpContext = new Moq.Mock<HttpContextBase>();
            httpContext.Setup(x => x.User).Returns(new CustomPrincipal(claims));
            httpContext.Setup(x => x.Session).Returns(session.Object);
            httpContext.Setup(x => x.Session["NotificationManager"]).Returns(sessionModel);
            var reqContext = new RequestContext(httpContext.Object, new RouteData());
            _controller = new NotificationsController();
            _controller.ControllerContext = new ControllerContext(reqContext, _controller);
        }

        #endregion
    }
}
