using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.CustomModels
{
    public class CreateEmployeeRequest
    {
            // Required: Employee name from client
            public string Name { get; set; } = string.Empty;

            // Required: Employee email from client
            public string Email { get; set; } = string.Empty;

            // Required: Department from client
            public string Department { get; set; } = string.Empty;

            // Required: Salary from client
            public decimal Salary { get; set; }
     }

    
}
