using System;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using Newtonsoft.Json;
using Portalia.Auth0.Constants;

namespace Portalia.Auth0.TokenGenerator
{
    public class Auth0TokenGenerator : ITokenGenerator
    {
        private static readonly MemoryCache Cache = MemoryCache.Default;

        public string GetManagementToken()
        {
            var token = Cache["auth0AccessToken"] as string;

            if (token == null)
            {
                var clientToken = new HttpClient { BaseAddress = new Uri(Auth0Constants.BaseUrl) };

                var content = new StringContent($"{{\"grant_type\":\"client_credentials\",\"client_id\": \"{Auth0Constants.ClientId}\",\"client_secret\": \"{Auth0Constants.ClientSecret}\",\"audience\": \"https://portalia.eu.auth0.com/api/v2/\"}}", Encoding.UTF8, "application/json");
                var res = clientToken.PostAsync("oauth/token", content).Result;

                var obj = new
                {
                    access_token = (string)null,
                    expires_in = 0,
                    scope = (string)null,
                    token_type = (string)null
                };

                var jwtToken = JsonConvert.DeserializeAnonymousType(res.Content.ReadAsStringAsync().Result, obj);

                token = jwtToken.access_token;
                Cache.Set("auth0AccessToken", token, new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(24)
                });
            }

            return token;
        }
    }
}