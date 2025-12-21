using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Data
{
    using Microsoft.EntityFrameworkCore;
    using EmployeeManagement.Model;


    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
            : base(options)
        {
        }

        //Mapping Employees table to employee class
        public DbSet<Employee> Employees => Set<Employee>();
    }
}
