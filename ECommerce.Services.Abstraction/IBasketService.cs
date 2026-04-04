using ECommerce.Domain.Contracts;
using ECommerce.Shared.DTOs.BasketDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Abstraction
{
    public interface IBasketService
    {
        // 🔹 Get Basket (create if not exists)
        Task<BasketDTO> GetBasketAsync(string userId);

        // 🔹 Unified operation: add / update / remove item
        Task<BasketDTO> UpdateItemQuantityAsync(string userId, int productId, int quantity);

        // 🔹 Update delivery method
        Task<BasketDTO> UpdateDeliveryMethodAsync(string userId, int deliveryMethodId);

        // 🔹 Delete basket
        Task<bool> DeleteBasketAsync(string userId);


    }
}
