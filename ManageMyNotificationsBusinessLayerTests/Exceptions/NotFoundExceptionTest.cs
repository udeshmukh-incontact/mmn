using ManageMyNotificationsBusinessLayer;
using NUnit.Framework;
using System;

namespace ManageMyNotificationsBusinessLayerTests.Exceptions
{
    [TestFixture]
    public class ServiceExceptionTest
    {
        [Test]
        public void Constructor_WithoutParameter_IsObjectCreated()
        {
            ServiceException serviceException = new ServiceException();
            Assert.IsNotNull(serviceException);
        }

        [Test]
        public void Constructor_WithMessage_IsObjectCreated()
        {
            ServiceException serviceException = new ServiceException("ServiceException");
            Assert.IsNotNull(serviceException);
            Assert.AreEqual(serviceException.Message, "ServiceException");
        }

        [Test]
        public void Constructor_WithMessageAndException_IsObjectCreated()
        {
            ServiceException serviceException = new ServiceException("ServiceException", new Exception("Inner Exception"));
            Assert.IsNotNull(serviceException);
            Assert.AreEqual(serviceException.Message, "ServiceException");
            Assert.AreEqual(serviceException.InnerException.Message, "Inner Exception");
        }
    }
}
