using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Interfaces;
using ManageMyNotificationsBusinessLayer.Services;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Net.Http;

namespace ManageMyNotificationsBusinessLayerTests.Services
{
    [TestFixture]
    public class CustomerNotificationsAPIServiceTests
    {
        private Mock<ICustomerNotificationsAPIHelper> mock_ICustomerNotificationsAPIHelper;
        private CustomerNotificationsAPIService _customerNotificationsAPIService;
        private readonly string TestInput = "Test";

        [SetUp]
        public void Setup()
        {
            mock_ICustomerNotificationsAPIHelper = new Mock<ICustomerNotificationsAPIHelper>();
            _customerNotificationsAPIService = new CustomerNotificationsAPIService(mock_ICustomerNotificationsAPIHelper.Object);
        }

        [Test]
        public void GetContactByAdfsGuidOnACR_NullInputTest()
        {
            var result = _customerNotificationsAPIService.GetContactByAdfsGuidOnACR(It.IsAny<string>());
            Assert.IsNotNull(result);
            Assert.IsNull(result.Id);
        }

        [Test]
        public void GetContactByAdfsGuidOnACR_SuccessTest()
        {
            SFContact sfContact = new SFContact();
            sfContact.Id = TestInput;
            mock_ICustomerNotificationsAPIHelper.Setup(x => x.CallApi<object, SFContact>("v2/contact/GetContactByAdfsGuid/Test", HttpMethod.Get, null)).ReturnsAsync(sfContact);
            var result = _customerNotificationsAPIService.GetContactByAdfsGuidOnACR(TestInput);
            Assert.IsNotNull(result);
            Assert.AreEqual(TestInput, result.Id);
        }

        [Test]
        public void GetContactByAdfsGuidOnACR_NullResultTest()
        {
            mock_ICustomerNotificationsAPIHelper.Setup(x => x.CallApi<object, SFContact>("v2/contact/GetContactByAdfsGuid/Test", HttpMethod.Get, null)).ReturnsAsync(new SFContact());
            var result = _customerNotificationsAPIService.GetContactByAdfsGuidOnACR(TestInput);
            Assert.IsNotNull(result);
            Assert.IsNull(result.Id);
        }

        [Test]
        public void GetTopLevelAccount_NullInputTest()
        {
            var result = _customerNotificationsAPIService.GetTopLevelAccount(It.IsAny<string>());
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetTopLevelAccount_SuccessTest()
        {
            List<string> output = new List<string> { TestInput };
            mock_ICustomerNotificationsAPIHelper.Setup(x => x.CallApi<string, List<string>>("v2/account/GetTopLevelAccountByAdfsGuid?adfsGuid=Test", HttpMethod.Get, null)).ReturnsAsync(output);
            var result = _customerNotificationsAPIService.GetTopLevelAccount(TestInput);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(TestInput, result[0]);
        }

        [Test]
        public void GetTopLevelAccount_EmptyResultTest()
        {
            mock_ICustomerNotificationsAPIHelper.Setup(x => x.CallApi<string, List<string>>("v2/account/GetTopLevelAccountByAdfsGuid?adfsGuid=Test", HttpMethod.Get, null)).ReturnsAsync(new List<string>());
            var result = _customerNotificationsAPIService.GetTopLevelAccount(TestInput);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void UpdateACRContact_NullInputTest()
        {
            var result = _customerNotificationsAPIService.UpdateACRContact(It.IsAny<string>(), It.IsAny<SFContact>());
            Assert.IsFalse(result);
        }

        [Test]
        public void UpdateACRContact_InputTest()
        {
            var obj = new List<dynamic>();
            obj.Add(new { value = "Test", path = "FirstName" });
            obj.Add(new { value = "Test", path = "LastName" });
            mock_ICustomerNotificationsAPIHelper.Setup(x => x.CallApi<List<dynamic>, List<string>>(It.IsAny<string>(), new HttpMethod("PATCH"), obj));
            var result = _customerNotificationsAPIService.UpdateACRContact("test", new SFContact() { FirstName = "Test", LastName = "Test" });
            Assert.IsFalse(result);
        }

    }
}
