using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities.BasketModule
{
    public class CustomerBasket
    {
        public string Id { get; set; } = default!;          // ممكن يكون UserId أو BasketId
        public List<BasketItem> Items { get; set; } = new();

        // اختياري حسب مشروعك:
        public int? DeliveryMethodId { get; set; }          // لو عايز تختار طريقة توصيل
        public decimal ShippingPrice { get; set; }          // سعر التوصيل
        public string? PaymentIntentID { get; set; }        // Stripe أو بوابة الدفع
        public string? ClientSecret { get; set; }           // للواجهة الأمامية لإكمال الدفع
    }
}
