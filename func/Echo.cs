// using Microsoft.Azure.Functions.Worker;
// using Microsoft.Extensions.Logging;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;

// namespace func
// {
//     public class Echo
//     {
//         private readonly ILogger<Echo> _logger;

//         public Echo(ILogger<Echo> logger)
//         {
//             _logger = logger;
//         }

//         [Function("Echo")]
//         public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
//         {
//             _logger.LogInformation("C# HTTP trigger function processed a request.");
//             return new OkObjectResult("Welcome to Azure Functions!");
//         }
//     }
// }


using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;    
namespace func
{
    public class Echo
    {
        private readonly ILogger _logger;
        public Echo(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Echo>();
        }
        [Function("Echo")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            StreamReader reader = new StreamReader(req.Body);
            string requestBody =await reader.ReadToEndAsync();
            await response.WriteStringAsync(requestBody);
            return response;
        }
    }
}