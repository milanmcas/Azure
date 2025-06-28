using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

namespace OIDCClient
{
    public class TokenService(IConfiguration configuration,IHttpClientFactory httpClientFactory, IConfidentialClientApplication confidentialClientApplication ) : ITokenService
    {
        private readonly IConfiguration _configuration= configuration??throw new ArgumentNullException(nameof(configuration));
        private readonly IHttpClientFactory _httpClientFactory=httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        private IConfidentialClientApplication _confidentialClient= confidentialClientApplication;
        
        //private IConfidentialClientApplication GetClient()
        //{
        //    return ConfidentialClientApplicationBuilder.Create(_configuration.GetSection("AzureAd:ClientId").Value)
        //    .WithClientSecret(_configuration.GetSection("AzureAd:ClientSecret").Value)
        //    .WithAuthority(AzureCloudInstance.AzurePublic, _configuration.GetSection("AzureAd:TenantId").Value)
        //    .Build();
        //}
        async Task<TokenResponse> ITokenService.GetTokenAsync()
        {
            using var _httpClient=_httpClientFactory.CreateClient();
            var form = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _configuration.GetSection("AzureAd:ClientId").Value??"" },
                { "client_secret", _configuration.GetSection("AzureAd:ClientSecret").Value??"" },
                { "scope", _configuration.GetSection("AzureAd:Scope").Value??"" }                
            };
            var request = new HttpRequestMessage(HttpMethod.Post, _configuration.GetSection("AzureAd:TokenUrl").Value)
            {
                Content = new FormUrlEncodedContent(form)
            };
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Token Response: " + json);
                var tokenJson = await GetAccessTokenAsync();
                Console.WriteLine("Token Response string: " + tokenJson);
                return JsonConvert.DeserializeObject<TokenResponse>(json)??new TokenResponse(); // Optionally deserialize and return the access_token
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error: " + error);
                throw new Exception("Token request failed");
            }
        }
        public async Task<string> GetAccessTokenAsync()
        {
            //_confidentialClient = GetClient();
            try
            {
                var result = await _confidentialClient.AcquireTokenForClient(new[] { _configuration.GetSection("AzureAd:Scope").Value })
                    .ExecuteAsync();
                return result.AccessToken;
            }
            catch (MsalException ex)
            {
                // Handle the exception appropriately (e.g., logging, retrying)
                Console.WriteLine($"Error acquiring token: {ex.Message}");
                throw;
            }
        }
    }
}
