using ECommerce.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.SalesDto
{
    public record UpdateSaleDto
    {
        public string? CustomerName { get; init; }
        public int? EmployeeId { get; init; }
        public int? ProductId { get; init; }
        public int? Quantity { get; init; }
        public SaleStatus? Status { get; init; }
    }
}
