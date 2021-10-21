using Elmah;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ManageMyNotificationsMVC.Common
{
    [ExcludeFromCodeCoverage]
    public class ElmahWrapper : IElmahWrapper

    {
        public void Raise(Exception ex)
        {
            if (ErrorSignal.FromCurrentContext() != null)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            else
            {
                ErrorLog.GetDefault(null).Log(new Error(ex));
            }
        }
    }
}