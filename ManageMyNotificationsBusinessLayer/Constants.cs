using System.Diagnostics.CodeAnalysis;

namespace ManageMyNotificationsBusinessLayer
{
    [ExcludeFromCodeCoverage]
    public static class Constants
    {
        public static class AuditType
        {
            public static readonly string ENABLED_NOTIFICATION_PROFILE = "Enabled Notification Profile";
            public static readonly string DISABLED_NOTIFICATION_PROFILE = "Disabled Notification Profile";
            public static readonly string ADDED_NOTIFICATION_DEVICE = "Added Notification Device";
            public static readonly string UPDATED_NOTIFICATION_DEVICE = "Updated Notification Device";
            public static readonly string REMOVED_NOTIFICATION_DEVICE = "Removed Notification Device";
            public static readonly string SUBSCRIBED_NOTIFICATION_GROUP = "Subscribed to Notification Group";
            public static readonly string UNSUBSCRIBED_NOTIFICATION_GROUP = "Unsubscribed to Notification Group";
            public static readonly string EDIT_XMATTERS_USERDETAILS = "Edit xMatters User Details";
            public static readonly string DEVICE_TIMEFRAME_CHANGED = "Device Timeframe Changed";
            public static readonly string ERROR = "Error";
        }

        public static class AuditLogMessage
        {
            public static readonly string MSG_PROFILE_ENABLED = "xMatters profile was enabled";
            public static readonly string MSG_PROFILE_DISABLED = "xMatters profile was disabled";
            public static readonly string MSG_ADDED_DEVICE = "{0} added to the user profile with the following value: {1}";
            public static readonly string MSG_UPDATED_DEVICE = "{0} changed from {1} to {2}";
            public static readonly string MSG_REMOVED_DEVICE = "{0} removed from the user profile with the following value: {1}";
            public static readonly string MSG_SUBSCRIBED_UNSUBSCRIBED = "User's {0} {1} to the following groups: {2}";
            public static readonly string MSG_EDIT_USERDETAILS = "User's {0} changed from {1} to {2}";
            public static readonly string MSG_TIMEFRAMECHANGED = "Changes made to user's device timeframe";
            public static readonly string MSG_ERROR = "The following error occurred when attempting to update the user profile: {0}";
        }

        public static class ErrorMessage
        {
            public static readonly string ADFS_SERVICE_DOWN = "The ADFS Service is either down or the url is incorrect.";
            public static readonly string SALESFORCE_SERVICE_DOWN = "The salesforce service is either down or the url is incorrect.";
            public static readonly string XMATTERS_SERVICE_DOWN = "The XMatters service is either down or the url is incorrect.";
        }

    }
}
