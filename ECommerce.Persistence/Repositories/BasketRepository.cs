using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Persistence.Data.DbContexts;
using ECommerce.Services.Abstraction;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Persistence.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly StoreDbContext _context;
        private readonly ICacheService _cache;

        public BasketRepository(StoreDbContext context, ICacheService cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        // ✅ Unified Cache Key
        private string GetKey(string basketId)
        {
            return $"basket_{basketId}";
        }

        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            return await _cache.GetAsync<CustomerBasket>(GetKey(basketId));
        }

        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(
            CustomerBasket basket,
            TimeSpan timeToLive = default)
        {
            var ttl = timeToLive == default
                ? TimeSpan.FromDays(1)
                : timeToLive;

            await _cache.SetAsync(
                GetKey(basket.UserId),
                basket,
                ttl
            );

            return basket;
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            await _cache.RemoveAsync(GetKey(basketId));
            return true;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products.FindAsync(productId);
        }
    }
}
