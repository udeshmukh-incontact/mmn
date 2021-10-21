using ManageMyNotificationsBusinessLayer;
using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Services.XMatters;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ManageMyNotificationsBusinessLayerTests.Services.XMatters
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class XMattersServiceHelperTests
    {

        [Test]
        public void CallXMatters_CallsUsingHttpGetMethod_AndReturnsValue()
        {
            var returnValue = "Test Result";
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("\"" + returnValue + "\""),
            };
            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new XMattersServiceHelper(httpClient);

            string result = helper.CallXMatters<string, string>("testUrl", HttpMethod.Get, null)?.Result;

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
        public void CallXMatters_CallsUsingHttpPostMethod_AndReturnsValue()
        {
            var returnValue = "Test Result";
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("\"" + returnValue + "\""),
            };
            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new XMattersServiceHelper(httpClient);
            var data = new Role() { Id = "2" };

            string result = helper.CallXMatters<Role, string>("testUrl", HttpMethod.Post, data)?.Result;

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
        public void CallXMatters_BaseUrl_Null_ThrowsServiceException()
        {
            var returnValue = "Test Result";
            var baseurl = System.Configuration.ConfigurationManager.AppSettings["xmattersBaseUrl"];
            System.Configuration.ConfigurationManager.AppSettings["xmattersBaseUrl"] = null;
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("\"" + returnValue + "\""),
            };
            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new XMattersServiceHelper(httpClient);

            string exceptionMessage = $"The XMatters service is either down or the url is incorrect.";

            Assert.That(() => helper.CallXMatters<string, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<ServiceException>());
            

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(0),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post && req.Content != null
               ),
               ItExpr.IsAny<CancellationToken>()
            );

            System.Configuration.ConfigurationManager.AppSettings["xmattersBaseUrl"] = baseurl;
        }

        [Test]
        public void CallXMatters_SendAsync_ThrowsAggregateException()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Default);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .Throws(new AggregateException())
               .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new XMattersServiceHelper(httpClient);

            string exceptionMessage = $"The XMatters service is either down or the url is incorrect.";

            Assert.That(() => helper.CallXMatters<string, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<AggregateException>());
            
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(0),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post && req.Content != null
               ),
               ItExpr.IsAny<CancellationToken>()
            );

        }

        [Test]
        public void CallXMatters_SendAsync_InnerException_ThrowsServiceException()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Default);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .Throws(new AggregateException(new HttpRequestException()))
               .Verifiable();
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new XMattersServiceHelper(httpClient);

            string exceptionMessage = $"The XMatters service is either down or the url is incorrect.";

            Assert.That(() => helper.CallXMatters<string, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<ServiceException>());
           
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(0),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post && req.Content != null
               ),
               ItExpr.IsAny<CancellationToken>()
            );

        }

        [Test]
        public void CallXMatters_SendAsync_ServiceUnavailable_ThrowsServiceException()
        {
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.ServiceUnavailable
            };

            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new XMattersServiceHelper(httpClient);

            string exceptionMessage = $"The XMatters service is either down or the url is incorrect.";

            Assert.That(() => helper.CallXMatters<string, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<ServiceException>());
            
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(0),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post && req.Content != null
               ),
               ItExpr.IsAny<CancellationToken>()
            );

        }

        [TestCase("400", typeof(FormatException), Description = "Throws Format Exception")]
        [TestCase("401", typeof(ServiceException), Description = "Throws Service Exception")]
        //[TestCase("409", typeof(AlreadyExistsException), Description = "Throws Already Exists Exception")]
        [TestCase("404", typeof(NotFoundException), Description = "Throws Not Found Exception")]
        [TestCase("425", typeof(Exception), Description = "Throws General Exception")]
        public void CallApi_CallsUsingHttpGetMethod_AndThrowsGeneralException(string code, Type exceptionType)
        {
            string message = "There is an error";
            var expectedResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{ 'code': '" + code + "','reason':'testreason', 'message': '" + message + "'}"),
            };

            var handlerMock = GetMockHandler(expectedResponse);
            var httpClient = new HttpClient(handlerMock.Object);
            var helper = new XMattersServiceHelper(httpClient);

            string exceptionMessage = $"XMatters returned status code: {code}, Message: {message}";
            if (exceptionType.FullName == typeof(FormatException).FullName)

                Assert.That(() => helper.CallXMatters<string, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<FormatException>());
            
            else if (exceptionType.FullName == typeof(ServiceException).FullName)

                Assert.That(() => helper.CallXMatters<string, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<ServiceException>());
           
            //else if (exceptionType.FullName == typeof(AlreadyExistsException).FullName)


            //    Assert.Throws<AlreadyExistsException>(() => { helper.CallXMatters<string, string>("testUrl", HttpMethod.Get, null); }, exceptionMessage);
            else if (exceptionType.FullName == typeof(NotFoundException).FullName)

                Assert.That(() => helper.CallXMatters<string, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<NotFoundException>());
           
            else if (exceptionType.FullName == typeof(Exception).FullName)

                Assert.That(() => helper.CallXMatters<string, string>("testUrl", HttpMethod.Get, null), Throws.TypeOf<Exception>());
           
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
