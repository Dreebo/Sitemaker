using System.Web;
using System.Web.Optimization;

namespace Sitemaker
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/dropzone").Include(
                        "~/Scripts/dropzone.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/createSlideShow").Include(
                        "~/Scripts/CreateSlideShow.js"));

            bundles.Add(new ScriptBundle("~/bundles/locationContent").Include(
                      "~/Scripts/locationContent.js"));

            bundles.Add(new ScriptBundle("~/bundles/loadTemplate").Include(
                      "~/Scripts/LoadTemplate.js",
                      "~/Scripts/MenuDroppable.js"));



            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/site.css",
                        "~/Content/basic.css",
                        "~/Content/dropzone.css"));

            bundles.Add(new StyleBundle("~/Content/Layout").Include(
                       "~/Content/Layout.css"));

            bundles.Add(new StyleBundle("~/Content/StyleGray").Include(
                        "~/Content/style_gray.css"));

            bundles.Add(new StyleBundle("~/Content/StyleWhight").Include(
                       "~/Content/style_white.css"));
        }
    }
}
