using ManageMyNotificationsBusinessLayer;
using NUnit.Framework;
using System;

namespace ManageMyNotificationsBusinessLayerTests.Exceptions
{
    [TestFixture]
    public class NotFoundExceptionTest
    {
        [Test]
        public void Constructor_WithoutParameter_IsObjectCreated()
        {
            NotFoundException notFoundException = new NotFoundException();
            Assert.IsNotNull(notFoundException);
        }

        [Test]
        public void Constructor_WithMessage_IsObjectCreated()
        {
            NotFoundException notFoundException = new NotFoundException("NotFoundException");
            Assert.IsNotNull(notFoundException);
            Assert.AreEqual(notFoundException.Message, "NotFoundException");
        }

        [Test]
        public void Constructor_WithMessageAndException_IsObjectCreated()
        {
            NotFoundException notFoundException = new NotFoundException("NotFoundException", new Exception("Inner Exception"));
            Assert.IsNotNull(notFoundException);
            Assert.AreEqual(notFoundException.Message, "NotFoundException");
            Assert.AreEqual(notFoundException.InnerException.Message, "Inner Exception");
        }
    }
}
