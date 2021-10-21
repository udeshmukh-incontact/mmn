using ManageMyNotificationsBusinessLayer.Interfaces;
using ManageMyNotificationsMVC.Controllers;
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
    public class ErrorControllerTest
    {
        private readonly string adfsGuid = "c18a5aa0-d576-447d-a563-44dcba28a509";
        private ErrorController _controller;
        private Mock<IPersonBusinessLogic> _personLogic;
        [SetUp]
        public void Setup()
        {
            _personLogic = new Mock<IPersonBusinessLogic>();
            InitController();
        }
        [Test]
        public void Index_View()
        {
            var actionResult = _controller.Index() as ActionResult;
            Assert.IsNotNull(actionResult);
        }

        [Test]
        public void Index_ExceptionTest()
        {
            _personLogic.Setup(x => x.GetTopLevelAccount(It.IsAny<string>())).Throws(new Exception());
            var actionResult = _controller.Index() as ViewResult;
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(actionResult.ViewData["ErrorMessage"]?.ToString());
        }

        [Test]
        public void NotAuthorized_View()
        {
            var actionResult = _controller.NotAuthorized() as ActionResult;
            Assert.IsNotNull(actionResult);
        }

        [Test]
        public void NotFound_View()
        {
            var actionResult = _controller.NotFound() as ActionResult;
            Assert.IsNotNull(actionResult);
        }
        [Test]
        public void IncompleteProfile_View()
        {
            var actionResult = _controller.IncompleteProfile() as ActionResult;
            Assert.IsNotNull(actionResult);
        }

        #region Private Methods
        private void InitController()
        {
            List<Claim> claims = new List<Claim>();
            Claim adfsGuidClaim = new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, Convert.ToBase64String(new Guid(adfsGuid).ToByteArray()));
            claims.Add(adfsGuidClaim);
            Claim userNameClaim = new Claim(System.Security.Claims.ClaimTypes.Name, "TestUserName");
            claims.Add(userNameClaim);

            var httpContext = new Moq.Mock<HttpContextBase>();
            httpContext.Setup(x => x.User).Returns(new CustomPrincipal(claims));
            var reqContext = new RequestContext(httpContext.Object, new RouteData());
            _controller = new ErrorController(_personLogic.Object);
            _controller.ControllerContext = new ControllerContext(reqContext, _controller);
        }

        #endregion
    }
}
