using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities.EmployeeModule
{
    public class Employee : BaseEntity<int>
    {
        public string Name { get; set; } 
        public string Email { get; set; }
        public string Phone { get; set; } 
        public string Position { get; set; } 
        public Department Department { get; set; }
        public decimal? Salary { get; set; }
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
        public DateTimeOffset JoinDate { get; set; } = DateTimeOffset.Now;
        public string? Permissions { get; set; }
    }
}
