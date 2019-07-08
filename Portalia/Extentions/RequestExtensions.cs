using System;
using System.Web;
using Newtonsoft.Json;
using Portalia.Core.Enum;
using Portalia.ViewModels;

namespace Portalia.Extentions
{
    public static class RequestExtensions
    {
        public static string GetRequestInfoInJson(this HttpRequestBase request, string username, string userId)
        {
            if (request == null)
            {
                return null;
            }

            var trackingActionDto = new RequestInfoModel
            {
                AccountId = userId,
                UserName = username,
                Url = request.Url.AbsoluteUri,
                CreatedDate = DateTime.UtcNow,
                ////Browser = request.GetBrowserBasicInfo(),
                ////IpAddress = request.UserHostAddress,
                TrackingActionType = TrackingActionType.HasAcceptedLegalPolicies.ToString()
            };

            return JsonConvert.SerializeObject(trackingActionDto);
        }

        private static BrowserModel GetBrowserBasicInfo(this HttpRequestBase request)
        {
            if (request == null)
            {
                return null;
            }

            return new BrowserModel
            {
                Name = request.Browser.Browser,
                Platform = request.Browser.Platform,
                Type = request.Browser.Type,
                Version = request.Browser.Version
            };
        }
    }
}