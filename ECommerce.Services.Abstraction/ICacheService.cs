using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Abstraction
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string cacheKey);

        Task SetAsync<T>(string cacheKey, T cacheValue, TimeSpan timeToLive);

        Task RemoveAsync(string cacheKey);
    }
}
