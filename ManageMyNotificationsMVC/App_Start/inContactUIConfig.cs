using System.Diagnostics.CodeAnalysis;
using System.Web.Optimization;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof(ManageMyNotificationsMVC.App_Start.inContactUIConfig), "Start")]

namespace ManageMyNotificationsMVC.App_Start
{
    [ExcludeFromCodeCoverage]
    public static class inContactUIConfig
    {        
        public static void Start(){
            BundleTable.Bundles.Add((new StyleBundle("~/Content/inContactUI/css").Include(
                                                   "~/Content/inContact.UI/2016.2/inContact.UI.boostrap.css",
                                                   "~/Content/inContact.UI/2016.2/inContact.UI.common.css",
                                                   "~/Content/inContact.UI/2016.2/inContact.UI.kendo.css",
                                                   "~/Content/inContact.UI/2016.2/inContact.UI.legacy.css",
                                                   "~/Content/inContact.UI/2016.2/inContact.UI.mvc.css")));

            BundleTable.Bundles.Add((new ScriptBundle("~/Scripts/inContactUI/common").Include(
                                                      "~/Scripts/inContact.UI/2016.2/inContact.UI.common.js")));

            BundleTable.Bundles.Add(new StyleBundle("~/Content/inContact.UI/2016.2/fontawesome").Include(
                                                    "~/Content/inContact.UI/2016.2/font-awesome.css"));

            BundleTable.Bundles.Add((new ScriptBundle("~/Content/inContactUI/spreadsheet").Include(
                 "~/Scripts/inContact.UI/2016.2/spreadsheet/inContact.UI.spreadsheet.js",
                 "~/Scripts/inContact.UI/2016.2/spreadsheet/ipVoiceSeatsValidation.js"
                )));
        }

    }
}
