using EmployeeManagement.Data;
using EmployeeManagement.EmployeBO;
using EmployeeManagement.EmployeeDAL;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;

var builder = FunctionsApplication.CreateBuilder(args);

// Enables HTTP triggers and middleware
builder.ConfigureFunctionsWebApplication();

// Register EF Core DbContext
builder.Services.AddDbContext<EmployeeDbContext>(options =>
{
    options.UseSqlServer(
        Environment.GetEnvironmentVariable("SqlConnectionString"));
});

//Register DAL
builder.Services.AddScoped<EmployeeDALC>();

//Register BO
builder.Services.AddScoped<EmployeeBO>(); 

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
