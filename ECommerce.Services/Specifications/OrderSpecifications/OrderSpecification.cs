using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.OrderModule;

namespace ECommerce.Services.Specifications.OrderSpecifications
{
    public class OrderSpecification : BaseSpecifications<Order, Guid>
    {
        public OrderSpecification(string userId)
            : base(O => O.UserId == userId)
        {
            AddInclude(X => X.DeliveryMethod);
            AddInclude(X => X.Items);
            AddOrderByDescending(X => X.OrderDate);
        }

        public OrderSpecification(Guid Id, string userId)
            : base(O => O.UserId == userId && O.Id == Id)
        {
            AddInclude(O => O.DeliveryMethod);
            AddInclude(O => O.Items);
        }
    }
}
