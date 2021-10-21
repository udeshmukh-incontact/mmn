using System;

namespace ManageMyNotificationsBusinessLayer
{
    public class ServiceException : Exception
    {
        public ServiceException() { }
        public ServiceException(string message) : base(message) { }

        public ServiceException(string message, Exception ex) : base(message, ex) { }
    }
}
