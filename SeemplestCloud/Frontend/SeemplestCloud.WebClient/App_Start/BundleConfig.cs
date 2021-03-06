﻿using System.Web.Optimization;

namespace SeemplestCloud.WebClient
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/core").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/angular.js",
                "~/Scripts/angular-route.js",
                "~/Scripts/angular-ui/ui-bootstrap.js",
                "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                "~/Scripts/moment.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/appInit").Include(
                "~/app/core/core.init.ng.js",
                "~/app/core/core.api.ng.js",
                "~/app/core/core.mainctrl.ng.js",
                "~/app/core/core.ng.js",
                "~/app/subscription/subscription.init.ng.js",
                "~/app/subscription/subscription.api.ng.js",
                "~/app/subscription/subscription.controllers.ng.js",
                "~/app/root.init.ng.js"
                ));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}
