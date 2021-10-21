using ManageMyNotificationsMVC.Controllers;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace ManageMyNotificationsMVCTests.Controllers
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class SessionTimeOutControllerTests
    {
        private SessionTimeOutController _controller;
        [SetUp]
        public void Setup()
        {
            _controller = new SessionTimeOutController();
        }

        [Test]
        public void Index_View()
        {
            var actionResult = _controller.Index() as ActionResult;
            Assert.IsNotNull(actionResult);
        }
    }
}
