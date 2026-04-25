using ECommerce.Domain.Entities.OrderModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Abstraction
{
    public interface IPaymobService
    {
        Task<string> GetPaymentKeyAsync(Order order);
    }
}
