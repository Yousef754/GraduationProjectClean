using ECommerce.Domain.Entities.OrderModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.OrderDTOs
{

    public record OrderToReturnDTO
    {
        public Guid Id { get; init; }
        public string UserId { get; init; }
        public ICollection<OrderItemDTO> Items { get; init; } = new List<OrderItemDTO>();

        public string Address { get; init; } = default!;
        public string Phone { get; init; } = default!;

        public DeliveryMethodDTO DeliveryMethod { get; init; } = default!;

        public PaymentMethod PaymentMethod { get; init; }

        public OrderStatus? Status { get; init; }
        public DateTimeOffset OrderDate { get; init; }

        public decimal Subtotal { get; init; }
        public decimal Total { get; init; }
    }
}
