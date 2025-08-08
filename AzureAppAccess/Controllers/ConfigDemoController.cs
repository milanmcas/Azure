using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureAppAccess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigDemoController : ControllerBase
    {
        private readonly IOptions<MyConfig> _config;

        public ConfigDemoController(IOptions<MyConfig> config)
            => _config = config;

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok(_config.Value);
        }
    }
}
