using System.Web;

namespace Portalia.Extentions
{
    public class SessionManager
    {
        public static bool IsShowBrowserWarning
        {
            get
            {
                if (HttpContext.Current.Session[SessionKeys.IsShowBrowserWarning] == null)
                    return true;
                return (bool)HttpContext.Current.Session[SessionKeys.IsShowBrowserWarning];
            }
            set
            {
                HttpContext.Current.Session[SessionKeys.IsShowBrowserWarning] = value;
            }
        }

        public static bool HasCheckedBrowser
        {
            get
            {
                if (HttpContext.Current.Session[SessionKeys.HasCheckedBrowser] == null)
                    return false;
                return (bool)HttpContext.Current.Session[SessionKeys.HasCheckedBrowser];
            }
            set
            {
                HttpContext.Current.Session[SessionKeys.HasCheckedBrowser] = value;
            }
        }
    }

    public class SessionKeys
    {
        public const string IsShowBrowserWarning= "IsShowBrowserWarning";
        public const string HasCheckedBrowser = "HasCheckedBrowser";
    }
}