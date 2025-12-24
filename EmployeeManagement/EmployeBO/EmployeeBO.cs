using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.CustomModels;
using EmployeeManagement.EmployeeDAL;
using EmployeeManagement.Model;
using EmployeeManagement.Validation;

namespace EmployeeManagement.EmployeBO
{
    public class EmployeeBO
    {
        // Field to hold DAL object (readonly = cannot be reassigned after constructor)
        private readonly EmployeeDALC _employeeDAL;

        // Constructor receives EmployeeDAL via Dependency Injection
        public EmployeeBO(EmployeeDALC employeeDAL)
        {
            _employeeDAL = employeeDAL;
        }

        /// <summary>
        /// Get all Employees
        /// </summary>
        /// <returns></returns>
        public async Task<List<Employee>> GetEmployees()
        {
            // Call DAL method to fetch employees from database
            return await _employeeDAL.GetEmployees();
        }

        /// <summary>
        /// Create employee method and validations performed
        /// </summary>
        /// <param name="createEmployeeRequest"></param>
        /// <returns></returns>
        public async Task<CreateEmployeeResult> CreateEmployee( CreateEmployeeRequest createEmployeeRequest)
        {
            CreateEmployeeResult result = new();
            Employee employee = new()
            {
                Email = createEmployeeRequest.Email,
                Name = createEmployeeRequest.Name,
                Salary = createEmployeeRequest.Salary,
                Department = createEmployeeRequest.Department

            };
            //Validations performed
            result.errors = EmployeeValidator.ValidateEmployee(employee);
            if(result.errors.Any())
            {
                result.isSuccess = false;
                return result ;
            }
            //Cal to DAL method for DB operations
            result.employees = await _employeeDAL.CreateEmployeeAsync(employee);

            result.isSuccess = true;
            return result;
            // Call DAL method to fetch employees from database
        }
    }
}
