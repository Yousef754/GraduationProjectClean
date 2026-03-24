using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.DTOs.OrderDTOs
{
    public record OrderDTO
    {
        public string BasketId { get; set; }

        public int DeliveryMethodId { get; set; }

        public AddressDTO ShipToAddress { get; set; }
    }
}
