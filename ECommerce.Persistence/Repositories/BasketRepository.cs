using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Persistence.Data.DbContexts;
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
        private readonly ICacheRepository _cache;

        public BasketRepository(StoreDbContext context, ICacheRepository cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        // ==========================
        // جلب السلة من CacheRepository
        // ==========================
        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            var data = await _cache.GetAsync(basketId);
            if (string.IsNullOrEmpty(data)) return null;

            return System.Text.Json.JsonSerializer.Deserialize<CustomerBasket>(data);
        }

        // ==========================
        // إنشاء أو تحديث السلة
        // ==========================
        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan timeToLive = default)
        {
            var ttl = timeToLive == default ? TimeSpan.FromDays(1) : timeToLive;
            var data = System.Text.Json.JsonSerializer.Serialize(basket);
            await _cache.SetAsync(basket.Id, data, ttl);
            return basket;
        }

        // ==========================
        // حذف السلة
        // ==========================
        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            var existing = await _cache.GetAsync(basketId);
            if (existing == null) return false;

            await _cache.SetAsync(basketId, string.Empty, TimeSpan.Zero);
            return true;
        }

        // ==========================
        // جلب منتج من DB
        // ==========================
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products.FindAsync(productId);
        }
    }
}
