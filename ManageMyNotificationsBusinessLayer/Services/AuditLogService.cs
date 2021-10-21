using ManageMyNotificationsBusinessLayer.Data;
using ManageMyNotificationsBusinessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ManageMyNotificationsBusinessLayer.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ICustomerNotificationsAPIHelper _apiHelper;
        private List<Message> _auditMessages = new List<Message>();
        public AuditLogService(ICustomerNotificationsAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public void AddToAuditLogMessageCollection(string type, string description)
        {
            Message m = new Message();
            m.AuditType = type;
            m.Description = description;
            _auditMessages.Add(m);
        }
        public void SaveAuditLogs(string adfsGuid, string userName, string xmGuid)
        {
            if (_auditMessages.Count == 0)
                return;
            AuditLogInfo aLog = new AuditLogInfo();
            aLog.ADFSGuid = adfsGuid;
            aLog.CreatedDate = DateTime.Now.ToUniversalTime();
            aLog.Source = "Manage My Notifications";
            aLog.UserName = userName;
            aLog.XmGUID = xmGuid;
            aLog.Messages = _auditMessages;

            string url = $"auditlog/create";
            _apiHelper.CallApi<AuditLogInfo, object>(url, HttpMethod.Post, aLog);
           _auditMessages.Clear();
        }
    }
}
