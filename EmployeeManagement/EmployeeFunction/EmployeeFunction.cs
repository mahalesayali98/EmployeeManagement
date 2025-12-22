using EmployeeManagement.Data;
using EmployeeManagement.Model;
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

    /// <summary>
    /// Method to get employees
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Method to add employees 
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Function("CreateEmployee")]
    public async Task<HttpResponseData> CreateEmployee([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Addemployees")]
    HttpRequestData req)
    {
        // 1️ Read request body JSON and convert it into Employee object
        Model.Employee employee = await req.ReadFromJsonAsync<Model.Employee>();

        // 2️ Validate request body
        if (employee == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid employee data");
            return badResponse;
        }

        // 3️ Add employee object to DbContext (invmemory tracking)
        employeeDbContext.Employees.Add(employee);

        // 4️ Save changes to database (INSERT query executed here)
        await employeeDbContext.SaveChangesAsync();

        // 5️ Create HTTP 201 (Created) response
        HttpResponseData response = req.CreateResponse(HttpStatusCode.Created);

        // 6️ Return newly created employee as JSON
        await response.WriteAsJsonAsync(employee);

        return response;
    }

    /// <summary>
    /// Method to Delete Employee
    /// </summary>
    /// <param name="req"></param>
    /// <param name="ID"></param>
    /// <returns></returns>
    [Function("DeleteEmployee")]
    public async Task<HttpResponseData> DeleteEmployee(
    [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "employees/{id:int}")]
    HttpRequestData req,
    int id)
    {
        // Find employee by primary key
        Employee employee = await employeeDbContext.Employees.FindAsync(id);

        // If employee not found, return 404
        if (employee == null)
        {
            HttpResponseData notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteStringAsync($"Employee ID : {id} not found.");
            return notFound;
        }

        // 3️ Add employee object to DbContext (invmemory tracking)
        employeeDbContext.Employees.Remove(employee);

        // 4️ Save changes to database (INSERT query executed here)
        await employeeDbContext.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync($"Employee with ID {id} is deleted.");

        return response;
    }

}
