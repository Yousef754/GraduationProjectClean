using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Shared;

namespace ECommerce.Services.Specifications.ProductSpecifications
{
    internal static class ProductSpecificationsHelper
    {
        public static Expression<Func<Product, bool>> GetCriteria(ProductQueryParams queryParams)
        {
            return p =>
                (!queryParams.CategoryId.HasValue || p.CategoryId == queryParams.CategoryId.Value)
                && (string.IsNullOrEmpty(queryParams.Search)
                    || p.Name.ToLower().Contains(queryParams.Search.ToLower())
                    || p.Description.ToLower().Contains(queryParams.Search.ToLower())
                );
        }
    }
}
