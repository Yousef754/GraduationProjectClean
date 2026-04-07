using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs
{
    public record DashboardDto
    {
        public int TotalEmployees { get; init; }
        public decimal TotalSalesAmount { get; init; }
        public int CompletedSalesCount { get; init; }
        public int PendingSalesCount { get; init; }
        public decimal TotalPurchasesAmount { get; init; }
        public int ReceivedOrdersCount { get; init; }
        public int PendingOrdersCount { get; init; }
        public int TotalProducts { get; init; }
        public int LowStockProducts { get; init; }
    }
}
