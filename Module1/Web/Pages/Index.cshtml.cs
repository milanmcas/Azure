using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private HttpClient _httpClient;
        private Options _options;
        private readonly IConfiguration _configuration;
        public IndexModel(HttpClient httpClient, Options options,IConfiguration configuration)
        {
            _httpClient = httpClient;
            _options = options;
            _configuration = configuration;
        }

        [BindProperty]
        public List<string> ImageList { get; private set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public async Task<string> GetTokenAsync(string clientId, string clientSecret, string tokenEndpoint)
        {
            var form = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "scope", "api://35f125ad-bfc4-4fee-9d2c-a3fb5b224402/.default" }

            };

            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
            {
                Content = new FormUrlEncodedContent(form)
            };

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Token Response: " + json);
                return json; // Optionally deserialize and return the access_token
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error: " + error);
                throw new Exception("Token request failed");
            }
        }
        public async Task OnGetAsync()
        {
            var imagesUrl = _options.ApiUrl;
            var tokenEndpoint = _configuration.GetSection("AzureAd:TokenUrl").Value; //"https://login.microsoftonline.com/8eb87a6e-8055-4135-b69d-f19c799ec045/oauth2/v2.0/token";
            var clientId = _configuration.GetSection("AzureAd:ClientId").Value; //"86a7c8da-a3dd-4b9b-8b95-81a1665d7454";
            var clientSecret = _configuration.GetSection("AzureAd:Secret").Value; //"j268Q~zow5dYZuM.5jiL2EVSjXsg1zZRlpUkUbCt";
            var result = await GetAccess_TokenAsync(clientId, clientSecret, tokenEndpoint);
            var response=JsonConvert.DeserializeObject<TokenResponse>(result);
            //var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IkNOdjBPSTNSd3FsSEZFVm5hb01Bc2hDSDJYRSJ9.eyJhdWQiOiIzNWYxMjVhZC1iZmM0LTRmZWUtOWQyYy1hM2ZiNWIyMjQ0MDIiLCJpc3MiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vOGViODdhNmUtODA1NS00MTM1LWI2OWQtZjE5Yzc5OWVjMDQ1L3YyLjAiLCJpYXQiOjE3NDk5MTY3MDYsIm5iZiI6MTc0OTkxNjcwNiwiZXhwIjoxNzQ5OTIwNjA2LCJhaW8iOiJrMlJnWUZoNzV5dUR3QlNEeGt2UGJudjZXMXB4TGRrMS8xa3BhOVFmejBDUE5xNmQyc3NCIiwiYXpwIjoiMzVmMTI1YWQtYmZjNC00ZmVlLTlkMmMtYTNmYjViMjI0NDAyIiwiYXpwYWNyIjoiMSIsIm9pZCI6IjczOTQxNTI1LTg3NzEtNDgzZC05YWQ3LTRjZmVlODE1NGY5MSIsInJoIjoiMS5BU2tBYm5xNGpsV0FOVUcybmZHY2VaN0FSYTBsOFRYRXYtNVBuU3lqLTFzaVJBSXBBQUFwQUEuIiwic3ViIjoiNzM5NDE1MjUtODc3MS00ODNkLTlhZDctNGNmZWU4MTU0ZjkxIiwidGlkIjoiOGViODdhNmUtODA1NS00MTM1LWI2OWQtZjE5Yzc5OWVjMDQ1IiwidXRpIjoiQldPYjVtZkJsazJnQktxWmlMMExBQSIsInZlciI6IjIuMCIsInhtc19mdGQiOiJmaDVXNjJOTHdOdDZMdzJIRkc0R0tyRXp2aTFhWm9RVGd1cGNYT2JKRFJzQmRYTmxZWE4wTFdSemJYTSJ9.ZX3N0myIpmda90FXjDjHfnlTyPec7jxVtld1A6MKdS0mUnQG_XO6XPxuzvkXyn4qekYaVW3Y1TP6M8q3Vu9Si3vNxBMaENabj9-2WfYoGz8kfkwsaRbfGiqbb0VhFR7qstgDwSEHUGtHl7E6WNJANRQ688iTV-0CE2GhKbQ59_McniCvgookcXTcUmlPjfOi3yObeqI-9132_-VMLGe7Y5mDPZALSTb4Q4S8gcG-19mw8KHcIttI0AH1Fq7j34CJzjU5xZnKMOZcmk8CJPpdUO1aRXQm1TK7j36eIe3Yvzb_M-_KG8U7veTcel65703N5ufJkX9OC_meBY82SqTt7w";
            //_httpClient.DefaultRequestHeaders.Add("Authorization",$"Bearer {response.AccessToken}");
            //var result = await GetAccess_TokenAsync(clientId,clientSecret, tokenEndpoint);// GetTokenAsync();
            //var result=await GetAccessTokenAsync(tokenEndpoint, clientId, clientSecret);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);
            string imagesJson = await _httpClient.GetStringAsync(imagesUrl);
            

            IEnumerable<string> imagesList = JsonConvert.DeserializeObject<IEnumerable<string>>(imagesJson);

            this.ImageList = imagesList.ToList<string>();
        }
        public async Task<string> GetAccessTokenAsync()
        {
            //var scopes = new[] { $"api://CLIENTID-API1/.default" };
            var scopes = new[] { $"api://86a7c8da-a3dd-4b9b-8b95-81a1665d7454/.default" };
            var authority = "https://login.microsoftonline.com/8eb87a6e-8055-4135-b69d-f19c799ec045/oauth2/v2.0/token";
            //var authority = $"https://login.microsoftonline.com/{_configuration["AzureAd:TenantId"]}/oauth2/v2.0/token";
            // Make sure you inject IConfiguration _configuration to read AppSettings.json values
            //var app = ConfidentialClientApplicationBuilder
            //    .Create(_configuration["AzureAd:ClientId"])
            //    .WithClientSecret(_configuration["AzureAd:ClientSecret"])
            //    .WithAuthority(authority)
            //    .Build();
            var clientId = "86a7c8da-a3dd-4b9b-8b95-81a1665d7454";
            var clientSecret = "j268Q~zow5dYZuM.5jiL2EVSjXsg1zZRlpUkUbCt";
            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(authority)                
                .Build();

            var result = await app.AcquireTokenForClient(scopes)
                .ExecuteAsync();

            return result.AccessToken;
        }
        public static async Task<string> GetAccessTokenAsync(string tokenEndpoint, string clientId, string clientSecret)
        {
            var scope = "api://35f125ad-bfc4-4fee-9d2c-a3fb5b224402/.default";
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"))
                );
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", scope }
            });

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                // Deserialize the response.  This assumes a simple JSON structure with an "access_token" property.
                //var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);
                return tokenResponse.AccessToken;
            }
        }
        public async Task<string> GetAccess_TokenAsync(string clientId, string clientSecret, string tokenEndpoint)
        {
            var form = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "scope", _configuration.GetSection("AzureAd:Scope").Value }
                //{ "scope", "api://86a7c8da-a3dd-4b9b-8b95-81a1665d7454/.default" }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
            {
                Content = new FormUrlEncodedContent(form)
            };

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Token Response: " + json);
                return json; // Optionally deserialize and return the access_token
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error: " + error);
                throw new Exception("Token request failed");
            }
        }
        
        public async Task<string> GetTokenAsync()
        {
            //var tokenService = new AzureTokenService();
            //var tokenJson = await tokenService.GetAccessTokenAsync(tenantId, clientId, clientSecret, scope);

            //// Optionally extract token
            //var token = System.Text.Json.JsonDocument.Parse(tokenJson)
            //    .RootElement
            //    .GetProperty("access_token")
            //    .GetString();
            var url = "https://login.microsoftonline.com/8eb87a6e-8055-4135-b69d-f19c799ec045/oauth2/v2.0/token";
            var requestBody = new
            {
                client_id = "86a7c8da-a3dd-4b9b-8b95-81a1665d7454",
                client_secret = "j268Q~zow5dYZuM.5jiL2EVSjXsg1zZRlpUkUbCt",
                grant_type = "client_credentials",
                scope = "api://86a7c8da-a3dd-4b9b-8b95-81a1665d7454/.default"
            };
            var request = JsonConvert.SerializeObject(requestBody);
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(requestBody));
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await _httpClient.PostAsync(url, stringContent);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var tokenEndpoint = "https://login.microsoftonline.com/8eb87a6e-8055-4135-b69d-f19c799ec045/oauth2/v2.0/token";
            var clientId = "86a7c8da-a3dd-4b9b-8b95-81a1665d7454";
            var clientSecret = "j268Q~zow5dYZuM.5jiL2EVSjXsg1zZRlpUkUbCt";
            var result = await GetAccess_TokenAsync(clientId, clientSecret, tokenEndpoint);
            var response1 = JsonConvert.DeserializeObject<TokenResponse>(result);
            if (Upload != null && Upload.Length > 0)
            {
                var imagesUrl = _options.ApiUrl;

                using (var image = new StreamContent(Upload.OpenReadStream()))
                {
                    image.Headers.ContentType = new MediaTypeHeaderValue(Upload.ContentType);
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {response1.AccessToken}");
                    var response = await _httpClient.PostAsync(imagesUrl, image);
                }
            }
            return RedirectToPage("/Index");
        }
    }
}