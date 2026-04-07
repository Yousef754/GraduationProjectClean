using ECommerce.Domain.Entities.EmployeeModule;
using ECommerce.Domain.Entities.ProductModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities.ParchaseModule
{
    public class Purchase : BaseEntity<int>
    {
        public string SupplierName { get; set; } = default!;
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = default!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public PurchaseStatus Status { get; set; } = PurchaseStatus.PendingOrder;
    }
}
