﻿namespace Portalia.Auth0.Models
{
    public class ManagementApiClientTokenResponse
    {
        public string access_token { get; internal set; }
        public string expires_in { get; internal set; }
        public string scope { get; internal set; }
        public string token_type { get; internal set; }
    }
}