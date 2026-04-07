using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.EmployeeDto
{
    public record EmployeeToReturnDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Phone { get; init; } = default!;
        public string Position { get; init; } = default!;
        public string Department { get; init; } = default!;
        public decimal Salary { get; init; }
        public string Status { get; init; } = default!;
        public DateTimeOffset JoinDate { get; init; }
        public string? Permissions { get; init; }
    }
}
