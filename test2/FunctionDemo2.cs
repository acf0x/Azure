using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace test2
{
    public class FunctionDemo2
    {
        private readonly ILogger<FunctionDemo2> _logger;

        public FunctionDemo2(ILogger<FunctionDemo2> logger)
        {
            _logger = logger;
        }

        [Function("FunctionDemo2")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
