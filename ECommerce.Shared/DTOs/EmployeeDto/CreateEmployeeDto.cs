using ECommerce.Domain.Entities.EmployeeModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.EmployeeDto
{
    public record CreateEmployeeDto
    {
        public string Name { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Phone { get; init; } = default!;
        public string Position { get; init; } = default!;
        public Department Department { get; init; }
        public decimal Salary { get; init; }
        public string? Permissions { get; init; }
    }
}
