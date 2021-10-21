using System;
using System.Collections.Generic;

namespace ManageMyNotificationsBusinessLayer.Data
{
    public class Message
    {
        public string AuditType { get; set; }
        public string Description { get; set; }
    }
    public class AuditLogInfo
    {
        public string ADFSGuid { get; set; }
        public string Source { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
        public string XmGUID { get; set; }
        public List<Message> Messages { get; set; }
    }
}
