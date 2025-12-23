using EmployeeManagement.Data;
using EmployeeManagement.Model;
using EmployeeManagement.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

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
            await (from emp in employeeDbContext.Employees
                   select emp).ToListAsync();
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
        EmployeeValidator employeeValidator = new();

        List<string> validationError = EmployeeValidator.ValidateEmployee(employee);

        // 2️ Validate request body
        List<string> errors = EmployeeValidator.ValidateEmployee(employee);

        if (errors.Any())
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(errors);
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
        // Employee employee = await employeeDbContext.Employees.FirstOrDefaultAsync(emp => emp.Id == id);

        // Using SQL query to find out id to delete
         Employee employee = await(from emp in employeeDbContext.Employees 
                                   where emp.Id > 1
                                   select emp)
                                   .FirstOrDefaultAsync();

        string error = EmployeeValidator.ValidateEmployeeID(id);
        // If employee not found, return 404
        if (error.Any())
        {
            HttpResponseData notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteAsJsonAsync(error);
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


    /// <summary>
    /// Method to update Employee
    /// </summary>
    /// <param name="req"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [Function("UpdateEmployee")]
    public async Task<HttpResponseData> UpdateEmployee( [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "employees/{id:int}")]
    HttpRequestData req,
    int id)
    {
        // Read request JSON
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        // Convert JSON to Employee
        Employee updatedEmployee = JsonSerializer.Deserialize<Employee>(
            requestBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Find existing employee
        //Model.Employee existingEmployee = await employeeDbContext.Employees
        //       .FirstOrDefaultAsync(e => e.Id == id);

        //Use of sql query instead sql

        Model.Employee existingEmployee = await (from emp in employeeDbContext.Employees
                                         where emp.Id > 1
                                         select emp)
                                        .FirstOrDefaultAsync();

        List<string> validationError = EmployeeValidator.ValidateEmployee(existingEmployee);
        if (validationError.Any())
        {
            HttpResponseData notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteAsJsonAsync(validationError);
            return notFound;
        }

        // Update fields
        existingEmployee.Name = updatedEmployee.Name;
        existingEmployee.Email = updatedEmployee.Email;
        existingEmployee.Department = updatedEmployee.Department;
        existingEmployee.Salary = updatedEmployee.Salary;

        // Save updates
        await employeeDbContext.SaveChangesAsync();

        HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(existingEmployee);

        return response;
    }

}
