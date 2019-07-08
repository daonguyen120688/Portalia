namespace Portalia.Core.Helpers
{
    public class StringHelpers
    {
        public static string GetEnvironmentUrl(string requestUrl)
        {
            if (string.IsNullOrEmpty(requestUrl))
            {
                return string.Empty;
            }

            if (requestUrl.IndexOf("https://inte.portalia.fr") >= 0)
            {
                return "https://inte.portalia.fr";
            }

            if (requestUrl.IndexOf("https://qa.portalia.fr") >= 0)
            {
                return "https://qa.portalia.fr";
            }

            if (requestUrl.IndexOf("https://portalia.fr") >= 0)
            {
                return "https://portalia.fr";
            }

            return string.Empty;
        }
    }
}
