using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace EmployeeManagement.EmployeeFunction;

public class EmployeeFunction
{
    private readonly ILogger<EmployeeFunction> _logger;
    private readonly DbContext dbContext;

    public EmployeeFunction(ILogger<EmployeeFunction> logger, DbContext DbContext)
    {
        dbContext = DbContext;
        _logger = logger;
    }

    [Function("GetEmployees")]
    public async Task<HttpResponseData> GetEmployees(
     [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "employees")]
    HttpRequestData req)
    {
       // var employees = await dbContext.Employees.ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
       // await response.WriteAsJsonAsync(employees);
        return response;
    }
}
