namespace ManageMyNotificationsBusinessLayer.Interfaces
{
    public interface IAuditLogService
    {
        void AddToAuditLogMessageCollection(string type, string description);
        void SaveAuditLogs(string adfsGuid, string userName, string xmGuid);
    }
}
