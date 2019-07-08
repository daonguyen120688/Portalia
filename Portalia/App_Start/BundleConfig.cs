using System.Web;
using System.Web.Optimization;

namespace Portalia
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalcustom").Include(
                        "~/Scripts/jquery.custom.validate.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/extension-method.js",//jquery extension method
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/headroom.min.js",
                      "~/Scripts/jQuery.headroom.min.js",
                      "~/Scripts/template.js",
                      "~/Scripts/respond.js"));


            bundles.Add(new ScriptBundle("~/bundles/UserManagement").Include(
                    "~/Scripts/UserManagement/user-management.js"));

            bundles.Add(new ScriptBundle("~/bundles/DocumentManagement").Include(
                    "~/Scripts/DocumentManagement/document-management.js"));

            bundles.Add(new ScriptBundle("~/bundles/Proposal").Include(
                    "~/Scripts/Proposal/proposal.js"));

            bundles.Add(new ScriptBundle("~/bundles/WelcomeCard").Include(
                  "~/Scripts/WelcomeCard/welcome-card.js"));

            bundles.Add(new ScriptBundle("~/bundles/UserProfile").Include(
                    "~/Scripts/userprofile/user-profile.js"));

            bundles.Add(new ScriptBundle("~/bundles/helper").Include(
                    "~/Scripts/helper/helper.js"));

            bundles.Add(new ScriptBundle("~/bundles/home").Include(
                    "~/Scripts/Home/home.js"));

            bundles.Add(new ScriptBundle("~/bundles/user").Include(
                "~/Scripts/user/register.js"));

            bundles.Add(new ScriptBundle("~/bundles/user-change-password").Include(
                "~/Scripts/user/changePasswordForNewPolicy.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/bootstrap-theme.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/site.css",
                      "~/Content/smart_wizard.css",
                      "~/Content/smart_wizard_theme_arrows.css",
                      "~/Content/smart_wizard_theme_circles.css",
                      "~/Content/smart_wizard_theme_dots.css",
                      "~/Content/Amaris.Document.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/dropzonescripts").Include(
                     //"~/Scripts/dropzone/dropzone.js",
                     "~/Scripts/dropzone-custom.js", "~/Scripts/Amaris.Document.js"));

            bundles.Add(new StyleBundle("~/Content/dropzonescss").Include(
                     "~/Scripts/dropzone/basic.css",
                     "~/Scripts/dropzone/dropzone.css"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                "~/Scripts/common.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/work-contract-css").Include(
                    "~/Content/work-contract.css"));

            bundles.Add(new ScriptBundle("~/bundles/work-contract").Include(
                "~/Scripts/bootstrap-datepicker.min.js",
                "~/Scripts/select2.min.js",
                "~/Scripts/WorkContract/work-contract.js"
            ));
#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
