using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.Persistence.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDatabase _database;

        public CacheRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
        }

        public async Task<string?> GetAsync(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey)) return null;

            var cacheValue = await _database.StringGetAsync(cacheKey);

            if (cacheValue.IsNullOrEmpty) return null;

            return cacheValue!;
        }

        public async Task SetAsync(string cacheKey, string value, TimeSpan timeToLive)
        {
            if (string.IsNullOrEmpty(cacheKey))
                throw new ArgumentNullException(nameof(cacheKey));

            await _database.StringSetAsync(cacheKey, value, timeToLive);
        }

        public async Task RemoveAsync(string cacheKey)
        {
            if (!string.IsNullOrEmpty(cacheKey))
                await _database.KeyDeleteAsync(cacheKey);
        }
    }
}
