using EmployeeManagement.Data;
using EmployeeManagement.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.EmployeeDAL
{
    public class EmployeeDALC
    {
        private EmployeeDbContext empDbContext;

        public EmployeeDALC(EmployeeDbContext employeeDbContext)
        {
            empDbContext = employeeDbContext;
        }
      
        /// Get all Employees
        /// </summary>
        /// <returns></returns>
        public async Task<List<Employee>> GetEmployees()
        {
            return (from emp in empDbContext.Employees
                    select emp).ToList();
        }


        // Method to insert a new employee record into the database
        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            // Adds the employee entity to EF Core change tracker
            // No database call happens here yet
            empDbContext.Employees.Add(employee);

            // Executes INSERT statement in the database
            // This is where the record is actually saved
            await empDbContext.SaveChangesAsync();

            // After SaveChangesAsync:
            // - Id is automatically populated (identity column)
            // - Entity reflects database state
            return employee;
        }

    }
}
