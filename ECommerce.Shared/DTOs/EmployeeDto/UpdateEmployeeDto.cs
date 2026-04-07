using ECommerce.Domain.Entities.EmployeeModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.EmployeeDto
{
    public record UpdateEmployeeDto
    {
        public string? Name { get; init; } 
        public string? Email { get; init; }
        public string? Phone { get; init; } 
        public string? Position { get; init; } 
        public Department? Department { get; init; }
        public decimal? Salary { get; init; }
        public EmployeeStatus? Status { get; init; }
        public string? Permissions { get; init; }
    }
}
