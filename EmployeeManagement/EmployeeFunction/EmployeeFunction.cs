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
    public async Task<HttpResponseData> GetEmployees([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "employees")]
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

    [Function("CreateEmployee")]
    public async Task<HttpResponseData> CreateEmployee( [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "employees")]
    HttpRequestData req)
    {
        // 1? Read request body JSON and convert it into Employee object
        Model.Employee employee = await req.ReadFromJsonAsync<Model.Employee>();

        // 2? Validate request body
        if (employee == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid employee data");
            return badResponse;
        }

        // 3? Add employee object to DbContext (invmemory tracking)
           employeeDbContext.Employees.Add(employee);

        // 4? Save changes to database (INSERT query executed here)
        await employeeDbContext.SaveChangesAsync();

        // 5? Create HTTP 201 (Created) response
        HttpResponseData response = req.CreateResponse(HttpStatusCode.Created);

        // 6? Return newly created employee as JSON
        await response.WriteAsJsonAsync(employee);

        return response;
    }


}
