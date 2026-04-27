using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using ECommerce.Services.Abstraction;

namespace ECommerce.Services
{
    public class CacheService : ICacheService
    {
        private readonly ICacheRepository _cacheRepository;
        private readonly JsonSerializerOptions _options;

        public CacheService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;

            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<T?> GetAsync<T>(string cacheKey)
        {
            var data = await _cacheRepository.GetAsync(cacheKey);
            if (string.IsNullOrEmpty(data))
                return default;

            if (typeof(T) == typeof(string))
                return (T)(object)data.Trim('"');

            return JsonSerializer.Deserialize<T>(data, _options);
        }

        public async Task SetAsync<T>(string cacheKey, T cacheValue, TimeSpan timeToLive)
        {
            var value = JsonSerializer.Serialize(cacheValue, _options);

            await _cacheRepository.SetAsync(cacheKey, value, timeToLive);
        }

        public async Task RemoveAsync(string cacheKey)
        {
            await _cacheRepository.RemoveAsync(cacheKey);
        }
    }
}
