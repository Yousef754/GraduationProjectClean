using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.BasketDTOs
{
    public record BasketDTO(
    string Id,
    List<BasketItemDTO> Items,
    int? DeliveryMethodId,
    decimal ShippingPrice,
    string? PaymentIntentID,
    string? ClientSecret
);
}
