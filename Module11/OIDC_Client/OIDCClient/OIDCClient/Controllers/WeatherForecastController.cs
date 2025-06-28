using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace OIDCClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;
        private Options _options;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, Options options, ITokenService tokenService,IHttpClientFactory httpClientFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenService= tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        [HttpGet("/token/response")]
        public async Task<IActionResult> GetResponse() {
            using var _httpClient = _httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",(await _tokenService.GetTokenAsync()).AccessToken);
            var result =await _httpClient.GetAsync(_options.ApiUrl);
            //result.EnsureSuccessStatusCode();
            if (result.IsSuccessStatusCode)
            {
                var output = await result.Content.ReadAsStringAsync();
                return Ok(output);
            }
            else
            {
                var error = await result.Content.ReadAsStringAsync();
                Console.WriteLine("Error: " + error);
                throw new Exception("Token request failed");
            }           
            
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
