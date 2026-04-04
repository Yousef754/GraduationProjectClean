using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.BasketDTOs
{
    public record BasketDTO(
    
    List<BasketItemDTO> Items,
    int? DeliveryMethodId,
    decimal ShippingPrice,
    string? PaymentIntentID,
    string? ClientSecret
)
    {
        public decimal Total =>
            Items.Sum(i => i.Price * i.Quantity) + ShippingPrice;
    }
}
