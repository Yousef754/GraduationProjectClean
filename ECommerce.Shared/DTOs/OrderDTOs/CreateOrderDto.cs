using ECommerce.Domain.Entities.OrderModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.OrderDTOs
{
    public record CreateOrderDto
    {
        public string Address { get; init; } = default!;
        public string Phone { get; init; } = default!;
        public int DeliveryMethodId { get; init; }
        public PaymentMethod PaymentMethod { get; init; }
    }
}
