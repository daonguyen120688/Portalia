using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Portalia.Core.Helpers
{
    public static class ApiRequestHelper
    {
        /// <summary>
        /// Invoke API with POST method asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of return object.</typeparam>
        /// <param name="host">Host name</param>
        /// <param name="url">API contract url</param>
        /// <param name="content">Post data</param>
        /// <returns>T type which is deserialized from JSON result</returns>
        public static async Task<T> PostAsync<T>(string host, string url, HttpContent content)
        {
            string responseString = string.Empty;
            string requestUrl = $"{GetBasedUrl(host)}/{url}";

            LoggerHelpers.Debug($"PostAsync - requestUrl:{requestUrl}");

            //Use Amaris account for authentication and add it to request header
            using (HttpClientHandler handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ApiUser"],
                        ConfigurationManager.AppSettings["ApiPwd"])

            })
            {
                using (var client = new HttpClient(handler))
                {
                    var response = await client.PostAsync(requestUrl, content);
                    responseString = await response.Content.ReadAsStringAsync();
                }
            }

            LoggerHelpers.Debug($"PostAsync - responseString:{responseString}");

            //Return immediately if T type is string
            if (typeof(T).Name == "String")
            {
                return (T)Convert.ChangeType(responseString, typeof(T));
            }

            T resultModel = JsonConvert.DeserializeObject<T>(responseString);
            return resultModel;
        }

        /// <summary>
        /// Get base url of Amaris API
        /// </summary>
        /// <param name="host">Host name of app</param>
        /// <returns></returns>
        private static string GetBasedUrl(string host)
        {
            string baseUrl = "https://arp.amaris.com";
            if (string.IsNullOrEmpty(host))
            {
                return baseUrl;
            }
            switch (host.Split('.')[0])
            {
                case "inte":
                    baseUrl = "https://inte.amaris.com";
                    break;
                case "qa":
                        baseUrl = "https://qaarp.amaris.com";
                        break;
            }

            LoggerHelpers.Debug($"GetBasedUrl - baseUrl:{baseUrl}----host:{host}");

            return baseUrl;
        }
    }
}
