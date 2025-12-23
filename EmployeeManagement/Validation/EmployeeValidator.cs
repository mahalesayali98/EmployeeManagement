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
        public static List<string> ValidateEmployee(List<Employee> employee)
        {
            var Errors = new List<string>();
            if (employee == null || !employee.Any())
            {
                Errors.Add("Employee Cannot be Null");
                return Errors;
            }
            foreach(Employee emp in employee)
            {
                if (string.IsNullOrEmpty(emp.Name)) ;
                Errors.Add("Name should not be empty");
                if(string.IsNullOrEmpty(emp.Email)) ;
                Errors.Add("Email should not be empty");
                if (string.IsNullOrEmpty(emp.Department)) ;
                Errors.Add("Department should not be empty");
                if (emp.Salary <= 0) ;
                Errors.Add("Salary is less than zero");
            }

            return Errors;
        }
    }
}
