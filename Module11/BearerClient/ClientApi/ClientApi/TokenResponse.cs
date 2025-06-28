using Newtonsoft.Json;

namespace ClientApi
{
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }
    }
}
