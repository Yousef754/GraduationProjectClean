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
        public string UserEmail { get; init; }
        public ICollection<OrderItemDTO> Items { get; init; }
        public AddressDTO ShipToAddress { get; init; }
        public DeliveryMethodDTO DeliveryMethod { get; init; }
        public string PaymentIntentId { get; set; }
        public OrderStatus Status { get; init; } // enum بدل string
        public DateTimeOffset OrderDate { get; init; }
        public decimal Subtotal { get; init; }
        public decimal Total { get; init; }
    }
}
