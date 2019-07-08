using System;
using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace Portalia.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ApiOutputCacheAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The time, in minutes, that the page or user control is cached.
        /// </summary>
        public int Duration { get; set; }

        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {
            filterContext.Response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.FromMinutes(Duration),
                MustRevalidate = true,
                Private = true
            };
        }
    }
}