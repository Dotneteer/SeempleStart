using System.Web.Optimization;

namespace Younderwater.Webclient
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/core").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/angular.js",
                "~/Scripts/angular-route.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/angular-ui/ui-bootstrap.js",
                "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                "~/Scripts/moment.js",
                "~/Scripts/bootstrap-opt-in.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/appInit").Include(
                "~/app/root/root.ng.js",
                "~/app/main/mainView.ng.js",
                "~/app/core/commonTypes.js",
                "~/app/core/commonDirectives.ng.js",
                "~/app/core/commonFilters.ng.js",
                "~/app/account/account.ng.dto.js",
                "~/app/core/ywapi.service.intf.js",
                "~/app/core/ywapi.service.js"
                ));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
