using EmployeeManagement.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

// Enables HTTP triggers and middleware
builder.ConfigureFunctionsWebApplication();

// Register EF Core DbContext
builder.Services.AddDbContext<EmployeeDbContext>(options =>
{
    options.UseSqlServer(
        Environment.GetEnvironmentVariable("SqlConnectionString"));
});

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
