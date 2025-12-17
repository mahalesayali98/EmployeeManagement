using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.EmployeeFunction;

public class EmployeeFunction
{
    private readonly ILogger<EmployeeFunction> _logger;

    public EmployeeFunction(ILogger<EmployeeFunction> logger)
    {
        _logger = logger;
    }

    [Function("Function1")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}