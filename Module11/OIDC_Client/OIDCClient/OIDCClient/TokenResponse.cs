using Newtonsoft.Json;

namespace OIDCClient
{
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }
    }
}
