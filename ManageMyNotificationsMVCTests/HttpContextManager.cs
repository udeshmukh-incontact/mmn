﻿using System;
using System.Web;

namespace ManageMyNotificationsMVCTests
{
    public class HttpContextManager
    {
        private static HttpContextBase m_context;
        public static HttpContextBase Current
        {
            get
            {
                if (m_context != null)
                    return m_context;

                if (HttpContext.Current == null)
                    throw new InvalidOperationException("HttpContext not available");

                return new HttpContextWrapper(HttpContext.Current);
            }
        }

        public static void SetCurrentContext(HttpContextBase context)
        {
            m_context = context;
        }
    }
}
