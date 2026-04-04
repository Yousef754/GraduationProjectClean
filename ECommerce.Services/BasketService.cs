using AutoMapper;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Persistence.Data.DbContexts;
using ECommerce.Services.Abstraction;
using ECommerce.Services.Exceptions;
using ECommerce.Shared.DTOs.BasketDTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ECommerce.Services
{
    public class BasketService : IBasketService
    {
        private readonly StoreDbContext _context;
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketService(IBasketRepository basketRepository, IMapper mapper,StoreDbContext context)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
            _context = context;

        }

        // =========================
        // Get Basket
        // =========================
        public async Task<BasketDTO> GetBasketAsync(string userId)
        {
            var basket = await _basketRepository.GetBasketAsync(userId);

            if (basket == null)
            {
                basket = new CustomerBasket
                {
                    UserId = userId,
                    Items = new List<BasketItem>()
                };

                await _basketRepository.CreateOrUpdateBasketAsync(basket);
            }

            return _mapper.Map<BasketDTO>(basket);
        }

        // =========================
        // Unified Add / Update / Remove
        // =========================
        public async Task<BasketDTO> UpdateItemQuantityAsync(string userId, int productId, int quantity)
        {
            if (quantity < 0)
                throw new Exception("Quantity cannot be negative");

            var basket = await _basketRepository.GetBasketAsync(userId)
                         ?? new CustomerBasket { UserId = userId, Items = new List<BasketItem>() };
            if (basket == null)
                basket = new CustomerBasket
                {
                    UserId = userId,
                    Items = new List<BasketItem>()
                };

            var product = await _basketRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product {productId} not found");

            var item = basket.Items.FirstOrDefault(x => x.ProductId == productId);

            if (quantity <= 0)
            {
                if (item != null)
                    basket.Items.Remove(item);
            }
            else
            {
                if (item == null)
                {
                    basket.Items.Add(new BasketItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        PictureUrl = product.PictureUrl,
                        Price = product.Price,
                        Quantity = quantity
                    });
                }
                else
                {
                    item.Quantity = quantity;
                }
            }

            await _basketRepository.CreateOrUpdateBasketAsync(basket);

            return _mapper.Map<BasketDTO>(basket);
        }

        // =========================
        // Delivery Method
        // =========================
        public async Task<BasketDTO> UpdateDeliveryMethodAsync(
            string userId,
            int deliveryMethodId
            )
        {
            var basket = await _basketRepository.GetBasketAsync(userId)
                         ?? throw new Exception("Basket not found");

            basket.DeliveryMethodId = deliveryMethodId;

            var method = await _context.DeliveryMethods
        .FirstOrDefaultAsync(x => x.Id == deliveryMethodId)
        ?? throw new Exception("Delivery method not found");

            basket.ShippingPrice = method.Price;

            await _basketRepository.CreateOrUpdateBasketAsync(basket);

            return _mapper.Map<BasketDTO>(basket);
        }

        // =========================
        // Delete Basket
        // =========================
        public async Task<bool> DeleteBasketAsync(string userId)
        {
            return await _basketRepository.DeleteBasketAsync(userId);
        }
    }
}
