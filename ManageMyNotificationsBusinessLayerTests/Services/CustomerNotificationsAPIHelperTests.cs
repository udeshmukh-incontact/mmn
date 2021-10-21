using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using Moq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using ManageMyNotificationsBusinessLayer.Services;
using Moq.Protected;
using System;
using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer;

namespace ManageMyNotificationsBusinessLayerTests.Services
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class CustomerNotificationsAPIHelperTests
    {
        [Test]
        public void CallApi_CallsUsingHttpGetMethod_AndReturnsValue()
        {
            var returnValue = "Test Result";
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("\"" + returnValue + "\""),
            };
            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new CustomerNotificationsAPIHelper(httpClient);

            string result = helper.CallApi<string, string>(It.IsAny<string>(), HttpMethod.Get)?.Result;

            Assert.That(result, Is.Not.Null);
            Assert.That(result == returnValue);

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public void CallApi_CallsUsingHttpPostMethod_AndReturnsValue()
        {
            var returnValue = "Test Result";
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("\"" + returnValue + "\""),
            };
            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new CustomerNotificationsAPIHelper(httpClient);
            var data = new AuditLogInfo() { ADFSGuid = "testGuid" };

            string result = helper.CallApi<AuditLogInfo, string>("testUrl", HttpMethod.Post, data)?.Result;

            Assert.That(result, Is.Not.Null);
            Assert.That(result == returnValue);

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post && req.Content != null
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public void CallApi_NotIsSuccessStatusCode_AndProcessError()
        {
            string code = "testCode";
            string message = "There is an error";
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadGateway,
                Content = new StringContent("{ 'code': '" + code + "','reason':'testreason', 'message': '" + message + "'}"),
            };
            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new CustomerNotificationsAPIHelper(httpClient);
            var data = new AuditLogInfo() { ADFSGuid = "testGuid" };

            Assert.That(() => helper.CallApi<AuditLogInfo, string>("testUrl", HttpMethod.Post, data), Throws.TypeOf<Exception>());

         

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post && req.Content != null
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public void CallApi_CallsUsingHttpGetMethod_AndThrowsException()
        {
            string code = "testCode";
            string message = "There is an error";
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{ 'code': '" + code + "','reason':'testreason', 'message': '" + message + "'}"),
            };

            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new CustomerNotificationsAPIHelper(httpClient);

            string exceptionMessage = $"CustomerNotificationsApi returned status code: {code}, Message: {message}";

            Assert.That(() => helper.CallApi<AuditLogInfo, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<Exception>());

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public void CallApi_CallsUsingHttpGetMethod_AndThrowsServiceException()
        {
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = new StringContent("No content")
            };

            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new CustomerNotificationsAPIHelper(httpClient);

            string exceptionMessage = "The CustomerNotificationsAPI service is either down or url is incorrect.";

            Assert.That(() => helper.CallApi<AuditLogInfo, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<ServiceException>());

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public void CallApi_CallsUsingHttpGetMethod_AndThrowsServiceExceptionOnAggregateException()
        {
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = new StringContent("No content")
            };
            HttpRequestException reEx = new HttpRequestException();
            AggregateException ex = new AggregateException("Aggregate Exception", reEx);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Default);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .Throws(ex)
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new CustomerNotificationsAPIHelper(httpClient);

            string exceptionMessage = "The CustomerNotificationsAPI service is either down or url is incorrect.";

            Assert.That(() => helper.CallApi<AuditLogInfo, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<ServiceException>());
            
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public void CallApi_CallsUsingHttpGetMethod_AndThrowsAggregateException()
        {
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.ServiceUnavailable,
                Content = new StringContent("No content")
            };
            ArgumentException reEx = new ArgumentException();
            AggregateException ex = new AggregateException("Aggregate Exception", reEx);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Default);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .Throws(ex)
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new CustomerNotificationsAPIHelper(httpClient);

            string exceptionMessage = "The CustomerNotificationsAPI service is either down or url is incorrect.";

            Assert.That(() => helper.CallApi<AuditLogInfo, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<AggregateException>());

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public void CallApi_CallsUsingHttpGetMethod_AndThrowsNotFoundException()
        {
            string code = "404";
            string message = "There is an error";
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{ 'code': '" + code + "','reason':'testreason', 'message': '" + message + "'}"),
            };

            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new CustomerNotificationsAPIHelper(httpClient);

            Assert.That(() => helper.CallApi<AuditLogInfo, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<NotFoundException>());

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public void CallApi_CallsUsingHttpGetMethod_AndThrowsServiceExceptionForXmatters()
        {
            string code = "503";
            string message = "There is an error";
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{ 'code': '" + code + "','reason':'testreason', 'message': '" + message + "'}"),
            };

            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new CustomerNotificationsAPIHelper(httpClient);

            Assert.That(() => helper.CallApi<AuditLogInfo, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<ServiceException>());

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public void CallApi_BaseUrl_Null_AndThrowsServiceExceptionForXmatters()
        {
            var baseurl = System.Configuration.ConfigurationManager.AppSettings["CustomerNotificationApiBaseUrl"];
            System.Configuration.ConfigurationManager.AppSettings["CustomerNotificationApiBaseUrl"] = null;

            string code = "503";
            string message = "There is an error";
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{ 'code': '" + code + "','reason':'testreason', 'message': '" + message + "'}"),
            };

            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new CustomerNotificationsAPIHelper(httpClient);
            Assert.That(() => helper.CallApi<AuditLogInfo, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<ServiceException>());

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(0),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
               ),
               ItExpr.IsAny<CancellationToken>()
            );

            System.Configuration.ConfigurationManager.AppSettings["CustomerNotificationApiBaseUrl"] = baseurl;
        }

        [Test]
        public void CallApi_CallsUsingHttpGetMethod_AndThrowsFormatException()
        {
            string code = "422";
            string message = "There is a format error";
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{ 'code': '" + code + "','reason':'testreason', 'message': '" + message + "'}"),
            };

            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new CustomerNotificationsAPIHelper(httpClient);

            Assert.That(() => helper.CallApi<AuditLogInfo, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<FormatException>());

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }



        #region Private Methods

        private Mock<HttpMessageHandler> GetMockHandler(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Default);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(response)
               .Verifiable();

            return handlerMock;
        }

        #endregion
    }
}
