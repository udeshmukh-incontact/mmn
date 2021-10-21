using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InContact.Common.Branding
{
    public class ServiceException : Exception
    {
        public ServiceException() { }
        public ServiceException(string message) : base(message) { }

        public ServiceException(string message, Exception ex) : base(message, ex) { }
    }
}