using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Services.Specifications
{
    public static class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            var query = inputQuery;

            // Apply filtering criteria
            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            // Apply includes (eager loading)
            query = spec.Includes.Aggregate(query, (current, include) =>
                current.Include(include));

            // Apply ordering
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            // Apply pagination
            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            return query;
        }
    }
}
