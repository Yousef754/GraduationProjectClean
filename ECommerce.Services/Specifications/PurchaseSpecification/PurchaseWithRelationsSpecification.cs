using ECommerce.Domain.Entities.ParchaseModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Specifications.PurchaseSpecification
{
    public class PurchaseWithRelationsSpecification : BaseSpecifications<Purchase, int>
    {
        public PurchaseWithRelationsSpecification() : base(p => true)
        {
            AddInclude(p => p.Employee);
            AddInclude(p => p.Product);
            AddInclude(p => p.Product.Category);
        }

        public PurchaseWithRelationsSpecification(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.Employee);
            AddInclude(p => p.Product);
            AddInclude(p => p.Product.Category);
        }
    }
}
