namespace Portalia.Auth0.Models
{
    public class ManagementApiClientTokenRequest
    {
        public string grant_type { get; internal set; }
        public string client_id { get; internal set; }
        public string client_secret { get; internal set; }
        public string audience { get; internal set; }
    }
}