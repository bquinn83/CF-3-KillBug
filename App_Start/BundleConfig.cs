using System.Web;
using System.Web.Optimization;

namespace KillBug
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery.js",
                        "~/Scripts/jquery.cookie.js",
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/datatables.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/popper.js",
                      "~/Scripts/bootstrap.js"
                      ));

            bundles.Add(new StyleBundle("~/css").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/font-awesome.min.css",
                      "~/Content/css/datatables.min.css",
                      "~/Content/css/font.css",
                      "~/Content/css/style.red.css",
                      "~/Content/css/custom.css"
                      ));
        }
    }
}