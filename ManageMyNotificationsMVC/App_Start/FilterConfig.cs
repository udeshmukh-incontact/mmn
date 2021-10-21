using ManageMyNotificationsMVC.Common;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace ManageMyNotificationsMVC
{
    [ExcludeFromCodeCoverage]
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new UnhandledErrorAttribute());
            //filters.Add(new HandleErrorAttribute());
        }
    }
}
