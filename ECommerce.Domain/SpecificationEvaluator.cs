using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity, Tkey>(
            IQueryable<TEntity> entryPoint,
            ISpecifications<TEntity, Tkey> specifications
        )
            where TEntity : BaseEntity<Tkey>
        {
            var Query = entryPoint; 

            if (specifications is not null)
            {
                if (specifications.Criteria is not null)
                {
                    Query = Query.Where(specifications.Criteria);
                }
                if (
                    specifications.IncludeExpressions is not null
                    && specifications.IncludeExpressions.Any()
                )
                {
                    Query = specifications.IncludeExpressions.Aggregate(
                        Query,
                        (CurrentQuery, includeExp) => CurrentQuery.Include(includeExp)
                    );
                }

                if (specifications.OrderBy is not null)
                {
                    Query = Query.OrderBy(specifications.OrderBy);
                }

                if (specifications.OrderByDescending is not null)
                {
                    Query = Query.OrderByDescending(specifications.OrderByDescending);
                }

                if (specifications.IsPaginated == true)
                {
                    Query = Query.Skip(specifications.Skip).Take(specifications.Take);
                }
            }

            return Query;
        }
    }
}
