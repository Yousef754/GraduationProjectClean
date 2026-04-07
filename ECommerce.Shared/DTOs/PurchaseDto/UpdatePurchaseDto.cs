using ECommerce.Domain.Entities.ParchaseModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.PurchaseDto
{
    public record UpdatePurchaseDto
    {
        public string? SupplierName { get; init; }
        public int? EmployeeId { get; init; }
        public int? ProductId { get; init; }
        public int? Quantity { get; init; }
        public decimal? Price { get; init; }
        public PurchaseStatus? Status { get; init; }
    }
}
