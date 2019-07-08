using System.Configuration;

namespace Portalia.Auth0.Constants
{
    public static class Auth0Constants
    {
        public static readonly string Domain = ConfigurationManager.AppSettings["ERP:Domain"];
        public static readonly string BaseUrl = $"https://{ConfigurationManager.AppSettings["ERP:Domain"]}/";
        public static readonly string ClientId = ConfigurationManager.AppSettings["ERP:ClientId"];
        public static readonly string ClientSecret = ConfigurationManager.AppSettings["ERP:ClientSecret"];
        public static readonly string Connection = ConfigurationManager.AppSettings["ERP:UserDatabaseConnection"];
        public static readonly string AdfsDomains = ConfigurationManager.AppSettings["ERP:EmployeeEmailDomains"];
        public static readonly string Scopes = "openid profile email";
        public const string AdfsConnection = "ADFS";
        public const string LinkedInConnection = "linkedin";
        public const string Auth0Connection = "auth0";

        public const string ConnectionTypeClaim = "https://erpapi/connection_type";
    }
}