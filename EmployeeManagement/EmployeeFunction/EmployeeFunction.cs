using EmployeeManagement.CustomModels;
using EmployeeManagement.Data;
using EmployeeManagement.EmployeBO;
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
    private readonly EmployeeBO employeeBO1;

    public EmployeeFunction(ILogger<EmployeeFunction> logger, EmployeeDbContext employeeDb,EmployeeBO employeeBo1)
    {
         this.employeeDbContext = employeeDb;
         employeeBO1 = employeeBo1;
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

        //Calling BO method To get list of employees
        Task<List<Employee>> ListOfEmployees= employeeBO1.GetEmployees();

        _logger.LogInformation("After DB call");

        HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

        await response.WriteAsJsonAsync(ListOfEmployees);
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
        CreateEmployeeRequest request =
         await req.ReadFromJsonAsync<CreateEmployeeRequest>();

        //Called BO to create Employee
        CreateEmployeeResult result =
            await employeeBO1.CreateEmployee(request);

        if (!result.isSuccess)
        {
            HttpResponseData badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(result.errors);
            return badResponse;
        }

        HttpResponseData response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(result);
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
        CreateEmployeeResult createEmployeeResult = new();
        // Find employee by primary key
        // Employee employee = await employeeDbContext.Employees.FirstOrDefaultAsync(emp => emp.Id == id);

        // Using SQL query to find out id to delete
        //Employee employee = await(from emp in employeeDbContext.Employees 
        //                          where emp.Id > 1
        //                          select emp)
        //                          .FirstOrDefaultAsync();

         createEmployeeResult =await employeeBO1.DeleteEmployee(id);
        // If employee not found, return 404
        if (!createEmployeeResult.isSuccess)
        {
            HttpResponseData notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteAsJsonAsync(createEmployeeResult.error);
            return notFound;

        }
        HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
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
