using ECommerce.Domain.Entities.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services.Specifications.SaleSpecific
{
    public class SaleWithRelationsSpecification : BaseSpecifications<Sale, int>
    {
        // GetAll
        public SaleWithRelationsSpecification() : base(s => true)
        {
            AddInclude(s => s.Employee);
            AddInclude(s => s.Product);
            AddInclude(s => s.Product.Category);
        }

        // GetById
        public SaleWithRelationsSpecification(int id) : base(s => s.Id == id)
        {
            AddInclude(s => s.Employee);
            AddInclude(s => s.Product);
            AddInclude(s => s.Product.Category);
        }
    }
}
