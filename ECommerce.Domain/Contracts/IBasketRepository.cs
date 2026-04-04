using ECommerce.Domain.Entities.BasketModule;
using ECommerce.Domain.Entities.ProductModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Contracts
{
    public interface IBasketRepository
    {
        Task<CustomerBasket?> GetBasketAsync(string basketId);

        Task<CustomerBasket?> CreateOrUpdateBasketAsync(
            CustomerBasket basket,
            TimeSpan timeToLive = default
        );

        Task<bool> DeleteBasketAsync(string basketId);

        Task<Product?> GetProductByIdAsync(int productId);
    }
}
