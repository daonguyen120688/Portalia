using System.Configuration;
using System.Web.Mvc;

namespace Portalia.Extentions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString GenerateCaptcha(this HtmlHelper helper)
        {

            var publicKey = ConfigurationManager.AppSettings.Get("ReCaptcha.PublicKey");
            var htmlInjectString = $"<div class=\"g-recaptcha\" data-sitekey=\"{publicKey}\" data-size=\"invisible\" data-badge=\"inline\" data-type=\"image\"></div>";
            return MvcHtmlString.Create(htmlInjectString);
        }
    }
}