using System.Diagnostics.CodeAnalysis;

namespace ManageMyNotificationsBusinessLayer.Data
{
    [ExcludeFromCodeCoverage]
    public class Errorresponse
    {
        string code { get; set; }
        string message { get; set; }
        string reason { get; set; }
        string subcode { get; set; }
    }
}
