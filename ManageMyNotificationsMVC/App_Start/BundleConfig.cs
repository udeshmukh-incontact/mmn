using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Web.Optimization;

namespace ManageMyNotificationsMVC
{
    [ExcludeFromCodeCoverage]
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
           // bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
           //        "~/Content/kendo/kendo.common.core.min.css",
           //        "~/Content/kendo/kendo.common.min.css",
           //        "~/Content/kendo/kendo.default.min.css",
           //        "~/Content/kendo/kendo.default.mobile.min.css"
           //));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/bootstrap.css")
                .Include("~/Content/bootstrap-datetimepicker.css")
                );//
            bundles.Add(new StyleBundle("~/Content/sitecss")
                .Include("~/Content/Site.css")
                );

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.cookie.js",
                        "~/Scripts/spin.js",
                        "~/Scripts/ajaxAPI.js")
                        .Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/moment.min.js",
                        "~/Scripts/bootstrap-checkbox.js",
                        "~/Scripts/bootstrap-datetimepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            //bundles.Add(new ScriptBundle("~/bundles/kendoui").Include(
            //            "~/Scripts/kendo.all.min.js",
            //            "~/Scripts/kendo.inContact.js"));

            bundles.Add(new ScriptBundle("~/bundles/incontact").Include(
                        "~/Scripts/incontact.export.js"));

            bundles.Add(new ScriptBundle("~/bundles/inContact.Spinner").Include(
                "~/scripts/incontact.spinner.js"));

            bundles.Add(new ScriptBundle("~/bundles/managenotification").Include(
                "~/Scripts/ie-polyfill.js",
                "~/Scripts/ManageNotificationValidation.js",
                "~/Scripts/ManageNotification.js"));

            bundles.Add(new ScriptBundle("~/bundles/sessiontimeout").Include(
                "~/Scripts/SessionTimeout.js"));

            bundles.Add(new ScriptBundle("~/bundles/foolproof").Include(
                        "~/Scripts/mvcfoolproof.unobtrusive.min.js",
                        "~/Scripts/MvcFoolproofJQueryValidation.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Code removed for clarity.
            BundleTable.EnableOptimizations = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableOptimizations"]);

            // Clear all items from the default ignore list to allow minified CSS and JavaScript files to be included in debug mode
            bundles.IgnoreList.Clear();

            // Add back the default ignore list rules sans the ones which affect minified files and debug mode
            bundles.IgnoreList.Ignore("*.intellisense.js");
            bundles.IgnoreList.Ignore("*-vsdoc.js");
            bundles.IgnoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
        }
    }
}
