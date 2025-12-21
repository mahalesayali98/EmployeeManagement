using EmployeeManagement.Data;
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
    private readonly EmployeeDbContext employeeDbContext;

    public EmployeeFunction(ILogger<EmployeeFunction> logger, EmployeeDbContext employeeDbContext)
    {
        this.employeeDbContext = employeeDbContext;
        _logger = logger;
    }

    [Function("GetEmployees")]
    public async Task<HttpResponseData> GetEmployees(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "employees")]
    HttpRequestData req)
    {
        _logger.LogInformation("GetEmployees called");

        _logger.LogInformation("Before DB call");

        List<Model.Employee> employees =
            await employeeDbContext.Employees.ToListAsync();

        _logger.LogInformation("After DB call");

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(employees);
        return response;
    }

}
