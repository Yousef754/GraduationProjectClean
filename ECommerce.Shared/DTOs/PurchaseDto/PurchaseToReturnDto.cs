using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.PurchaseDto
{
    public record PurchaseToReturnDto
    {
        public int Id { get; init; }
        public string SupplierName { get; init; } = default!;
        public string EmployeeName { get; init; } = default!;
        public string ProductName { get; init; } = default!;
        public string CategoryName { get; init; } = default!;
        public int Quantity { get; init; }
        public decimal Price { get; init; }
        public decimal TotalAmount { get; init; }
        public string Status { get; init; } = default!;
    }
}
