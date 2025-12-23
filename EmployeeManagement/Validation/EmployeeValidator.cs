using EmployeeManagement.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Validation
{
    public class EmployeeValidator
    {
        /// <summary>
        /// Validate Employee method 
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static List<string> ValidateEmployee(Employee employee)
        {
            List<string> Errors = new List<string>();
            if (employee == null)
            {
                Errors.Add("Employee Cannot be Null");
                return Errors;
            }
           
                if (string.IsNullOrEmpty(employee.Name)) 
                Errors.Add("Name should not be empty");
                if(string.IsNullOrEmpty(employee.Email)) 
                Errors.Add("Email should not be empty");
                if (string.IsNullOrEmpty(employee.Department)) 
                Errors.Add("Department should not be empty");
                if (employee.Salary <= 0) 
                Errors.Add("Salary is less than zero");    

            return Errors;
        }

        public static string? ValidateEmployeeID(int id)
        {
            if (id <= 0)
            {
                return "Employee ID must be greater than zero";
            }

            return null; // valid ID
        }


    }
}
