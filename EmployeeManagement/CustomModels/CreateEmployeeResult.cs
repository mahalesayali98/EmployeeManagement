using EmployeeManagement.Model;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.CustomModels
{
    public class CreateEmployeeResult
    {
        public bool isSuccess { get;  set; }
        public List<string> errors { get;  set; }

        public string error { get; set; }
        public Employee employees { get; set; }
    }
}
