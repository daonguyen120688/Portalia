using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Portalia.App_Start;
using Portalia.Core.Interface.Service;

namespace Portalia.Filter
{
    public class SecuredTableAttribute : AuthorizeAttribute
    {
        public string TableName { get; set; }
        public string KeyName { get; set; }
        public string KeyValue { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }
            var securedTableService = UnityConfig.GetConfiguredContainer().Resolve<ISecuredTable>();
            if (httpContext.User.IsInRole("Administrator"))
            {
                return true;
            }
            var currentUserId = httpContext.User.Identity.GetUserId();
            var userId = httpContext.Request.Params[KeyValue];
            return securedTableService.HavePermission(currentUserId, TableName, KeyName);
        }
    }
}