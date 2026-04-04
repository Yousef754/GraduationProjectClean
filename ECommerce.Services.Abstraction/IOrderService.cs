using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Shared.CommonResponses;
using ECommerce.Shared.DTOs.OrderDTOs;

namespace ECommerce.Services.Abstraction
{
    public interface IOrderService
    {
        Task<Result<OrderToReturnDTO>> CreateOrderAsync(CreateOrderDto dto, string userEmail);

        Task<Result<IEnumerable<DeliveryMethodDTO>>> GetAllDeliveryMethodsAsync();

        Task<Result<IEnumerable<OrderToReturnDTO>>> GetAllOrdersAsync(string userEmail);

        Task<Result<OrderToReturnDTO>> GetOrderByIdAsync(Guid id, string userEmail);
    }
}
