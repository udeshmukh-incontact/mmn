using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using ManageMyNotificationsBusinessLayer.Data;
using Moq;
using ManageMyNotificationsBusinessLayer.Services;
using ManageMyNotificationsBusinessLayer.Interfaces;
using System.Net.Http;

namespace ManageMyNotificationsBusinessLayerTests.Services
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class AuditLogServiceTests
    {
        private Mock<ICustomerNotificationsAPIHelper> _helper;
        private AuditLogService _auditService;
        private string _type, _message, _url, _adfsGuid, _userName, _xmGuid;

       [SetUp]
        public void Setup()
        {
            _helper = new Mock<ICustomerNotificationsAPIHelper>();
            _auditService = new AuditLogService(_helper.Object);
            _type = "testType";
            _message = "TestMessage";
            _url = "auditlog/create";
            _adfsGuid = "testAdfsGuid";
            _userName = "testUserName";
            _xmGuid = "testXMGuid";

        }

        [Test]
        public void SaveAuditLogs_Calls_HelperApi()
        {
            _auditService.AddToAuditLogMessageCollection(_type, _message);
            _helper.Setup(x => x.CallApi<AuditLogInfo, object>(_url, HttpMethod.Post, It.IsAny<AuditLogInfo>()));
            _auditService.SaveAuditLogs(_adfsGuid, _userName, _xmGuid);
            _helper.Verify(x => x.CallApi<AuditLogInfo, object>(_url, HttpMethod.Post, It.IsAny<AuditLogInfo>()), Times.Once);
        }

        [Test]
        public void SaveAuditLogs_DoesNotCall_HelperApi()
        {
            Mock<ICustomerNotificationsAPIHelper> helper = new Mock<ICustomerNotificationsAPIHelper>();
            AuditLogService auditService = new AuditLogService(helper.Object);
            helper.Setup(x => x.CallApi<AuditLogInfo, object>(_url, HttpMethod.Post, It.IsAny<AuditLogInfo>()));
            auditService.SaveAuditLogs(_adfsGuid, _userName, _xmGuid);
            helper.Verify(x => x.CallApi<AuditLogInfo, object>(_url, HttpMethod.Post, It.IsAny<AuditLogInfo>()), Times.Never);
        }
    }
}
