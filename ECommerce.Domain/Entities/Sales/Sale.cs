using ECommerce.Domain.Entities.EmployeeModule;
using ECommerce.Domain.Entities.ProductModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities.Sales
{
    public class Sale : BaseEntity<int>
    {
        public string CustomerName { get; set; } = default!;
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = default!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public SaleStatus Status { get; set; } = SaleStatus.Completed;
    }
    
}
