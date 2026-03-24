using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using StackExchange.Redis;

namespace ECommerce.Persistence.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDatabase _database;

        public CacheRepository(IConnectionMultiplexer connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            _database = connection.GetDatabase();
        }

        public async Task<string?> GetAsync(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey)) return null;

            var cacheValue = await _database.StringGetAsync(cacheKey);
            return cacheValue.IsNullOrEmpty ? null : cacheValue.ToString();
        }

        public async Task SetAsync(string cacheKey, string cacheValue, TimeSpan timeToLive)
        {
            if (string.IsNullOrEmpty(cacheKey)) throw new ArgumentNullException(nameof(cacheKey));
            if (cacheValue == null) throw new ArgumentNullException(nameof(cacheValue));

            await _database.StringSetAsync(cacheKey, cacheValue, timeToLive);
        }

        public async Task RemoveAsync(string cacheKey)
        {
            await _database.KeyDeleteAsync(cacheKey);
        }
    }
}
