using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Exceptions;
using ECommerce.Shared.DTOs.BasketDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketService(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        // 🔹 إنشاء أو تحديث السلة وإضافة منتجات
        public async Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basketDto)
        {
            var basket = new CustomerBasket
            {
                Id = basketDto.Id,
                Items = new List<BasketItem>()
            };

            foreach (var item in basketDto.Items)
            {
                var product = await _basketRepository.GetProductByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found");

                basket.Items.Add(new BasketItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    PictureUrl = product.PictureUrl,
                    Price = product.Price,
                    Quantity = item.Quantity
                });
            }

            await _basketRepository.CreateOrUpdateBasketAsync(basket);
            return _mapper.Map<BasketDTO>(basket);
        }

        // 🔹 جلب السلة حسب الـ Id
        public async Task<BasketDTO> GetBasketAsync(string basketId)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null)
                throw new Exception("Basket not found");

            return _mapper.Map<BasketDTO>(basket);
        }

        // 🔹 حذف السلة
        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _basketRepository.DeleteBasketAsync(basketId);
        }

        // 🔹 إضافة منتج للسلة
        public async Task<BasketDTO> AddProductToBasketAsync(string basketId, int productId, int quantity)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId)
                         ?? new CustomerBasket { Id = basketId };

            var product = await _basketRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new Exception($"Product with ID {productId} not found");

            var existingItem = basket.Items.FirstOrDefault(x => x.ProductId == productId);
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
                basket.Items.Add(new BasketItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    PictureUrl = product.PictureUrl,
                    Price = product.Price,
                    Quantity = quantity
                });

            await _basketRepository.CreateOrUpdateBasketAsync(basket);
            return _mapper.Map<BasketDTO>(basket);
        }

        // 🔹 تحديث DeliveryMethod وحساب shippingPrice باستخدام UnitOfWork
        public async Task<BasketDTO> UpdateDeliveryMethodAsync(string basketId, int deliveryMethodId, IUnitOfWork unitOfWork)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId)
                         ?? throw new Exception("Basket not found");

            basket.DeliveryMethodId = deliveryMethodId;

            // 🔹 جلب DeliveryMethod من UnitOfWork وليس BasketRepository
            var method = await unitOfWork.GetRepository<DeliveryMethod, int>()
                                         .GetByIdAsync(deliveryMethodId)
                         ?? throw new Exception("DeliveryMethod not found");

            basket.ShippingPrice = method.Price;

            await _basketRepository.CreateOrUpdateBasketAsync(basket);
            return _mapper.Map<BasketDTO>(basket);
        }
    }
}
