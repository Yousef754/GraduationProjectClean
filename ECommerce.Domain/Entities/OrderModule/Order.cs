using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities.OrderModule
{
    public class Order : BaseEntity<Guid>
    {
        public string UserId { get; set; } = default!;

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

        public string Phone { get; init; } = default!;


        public OrderStatus? Status { get; set; } = OrderStatus.Pending;

        public PaymentMethod PaymentMethod { get; set; } // 👈 الجديد
        public string? PaymentIntentId { get; set; }

        public string Address { get; set; } = default!;

        public DeliveryMethod DeliveryMethod { get; set; } = default!;
        public int DeliveryMethodId { get; set; }

        public ICollection<OrderItem> Items { get; set; } = [];

        public decimal SubTotal { get; set; }

        public decimal GetTotal() => SubTotal + DeliveryMethod.Price;
    }
}
