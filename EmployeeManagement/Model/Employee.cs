using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Model
{
   public class Employee
    {

            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public decimal Salary { get; set; }
    }
}
