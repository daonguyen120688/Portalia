using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Portalia.API.Attributes
{
    public class AuthorizeApiAttribute : AuthorizeAttribute
    {
        private static readonly string ApiKeyName = ConfigurationManager.AppSettings.Get("API.ApiKeyName");
        private static readonly string ApiKeyValue = ConfigurationManager.AppSettings.Get("API.ApiKeyValue");
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            logger.Debug($"AuthorizeApiAttribute - ApiKeyName:{ApiKeyName}");
            logger.Debug($"AuthorizeApiAttribute - ApiKeyValue:{ApiKeyValue}");

            

            foreach(var header in actionContext.Request.Headers)
            {
                string value = string.Join(",",header.Value);
                logger.Debug($"AuthorizeApiAttribute - Header key:{header.Key}");
                logger.Debug($"AuthorizeApiAttribute - Header value:{ value}");
            }

            if (actionContext.Request.Headers.Contains(ApiKeyName) &&
                actionContext.Request.Headers.FirstOrDefault(x => x.Key == ApiKeyName).Value.FirstOrDefault() == ApiKeyValue)
            {
                return;
            }

            logger.Debug($"AuthorizeApiAttribute - Authenticate failure");

            actionContext.Response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
                Content = new StringContent("You don't have access to this page. Please contact our support team if it should be the case.")
            };
        }
    }
}