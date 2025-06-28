using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace ClientApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController(ILogger<WeatherForecastController> logger,Options options, ITokenService tokenService, IHttpClientFactory httpClientFactory) : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger= logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ITokenService _tokenService=tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        private readonly IHttpClientFactory _httpClientFactory=httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        private Options _options= options ?? throw new ArgumentNullException(nameof(options));

        [HttpGet("/token/response")]
        public async Task<IActionResult> GetResponse()
        {
            using var _httpClient = _httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (await _tokenService.GetTokenAsync()).AccessToken);
            var result = await _httpClient.GetAsync(_options.ApiUrl);
            result.EnsureSuccessStatusCode();
            var output = await result.Content.ReadAsStringAsync();
            return Ok(output);
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
