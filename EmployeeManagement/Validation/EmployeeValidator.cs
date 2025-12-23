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
    }
}
