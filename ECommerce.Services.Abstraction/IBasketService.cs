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
        // 🔹 إنشاء أو تحديث السلة وإضافة منتجات
        Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basketDto);

        // 🔹 جلب السلة حسب الـ Id
        Task<BasketDTO> GetBasketAsync(string basketId);

        // 🔹 حذف السلة
        Task<bool> DeleteBasketAsync(string basketId);

        // 🔹 إضافة منتج للسلة
        Task<BasketDTO> AddProductToBasketAsync(string basketId, int productId, int quantity);

        // 🔹 تحديث DeliveryMethod وحساب shippingPrice (باستخدام UnitOfWork)
        Task<BasketDTO> UpdateDeliveryMethodAsync(string basketId, int deliveryMethodId, IUnitOfWork unitOfWork);
    }
}
