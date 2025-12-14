using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Shared.DTOS.Pagination;

namespace Services.Specifications
{
    public class CarWithFiltersSpecification : BaseSpecification<Car>
    {
        public CarWithFiltersSpecification(CarQueryParameters parameters)
            : base(BuildCriteria(parameters))
        {
            // Apply sorting
            ApplySorting(parameters);

            // Apply pagination
            ApplyPaging(
                (parameters.PageNumber - 1) * parameters.PageSize,
                parameters.PageSize
            );
        }

        private static Expression<Func<Car, bool>> BuildCriteria(CarQueryParameters parameters)
        {
            // Start with base criteria: not deleted
            Expression<Func<Car, bool>> criteria = c => !c.IsDeleted;

            // Brand filter
            if (!string.IsNullOrWhiteSpace(parameters.Brand))
            {
                var brand = parameters.Brand.ToLower();
                criteria = criteria.And(c => c.Brand.ToLower() == brand);
            }

            // Search term (Name or Brand contains)
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                criteria = criteria.And(c =>
                    c.Name.ToLower().Contains(searchTerm) ||
                    c.Brand.ToLower().Contains(searchTerm));
            }

            // Price range filters
            if (parameters.MinPrice.HasValue)
            {
                criteria = criteria.And(c => c.PricePerDay >= parameters.MinPrice.Value);
            }

            if (parameters.MaxPrice.HasValue)
            {
                criteria = criteria.And(c => c.PricePerDay <= parameters.MaxPrice.Value);
            }

            // Availability filter
            if (parameters.Available.HasValue)
            {
                criteria = criteria.And(c => c.Available == parameters.Available.Value);
            }

            return criteria;
        }

        private void ApplySorting(CarQueryParameters parameters)
        {
            switch (parameters.SortBy?.ToLower())
            {
                case "name":
                    if (parameters.SortDescending)
                        ApplyOrderByDescending(c => c.Name);
                    else
                        ApplyOrderBy(c => c.Name);
                    break;

                case "brand":
                    if (parameters.SortDescending)
                        ApplyOrderByDescending(c => c.Brand);
                    else
                        ApplyOrderBy(c => c.Brand);
                    break;

                case "price":
                    if (parameters.SortDescending)
                        ApplyOrderByDescending(c => c.PricePerDay);
                    else
                        ApplyOrderBy(c => c.PricePerDay);
                    break;

                case "createdat":
                    if (parameters.SortDescending)
                        ApplyOrderByDescending(c => c.CreatedAt);
                    else
                        ApplyOrderBy(c => c.CreatedAt);
                    break;

                default:
                    ApplyOrderBy(c => c.Id);
                    break;
            }
        }
    }

    public class CarWithFiltersForCountSpecification : BaseSpecification<Car>
    {
        public CarWithFiltersForCountSpecification(CarQueryParameters parameters)
            : base(BuildCriteria(parameters))
        {
        }

        private static Expression<Func<Car, bool>> BuildCriteria(CarQueryParameters parameters)
        {
            Expression<Func<Car, bool>> criteria = c => !c.IsDeleted;

            if (!string.IsNullOrWhiteSpace(parameters.Brand))
            {
                var brand = parameters.Brand.ToLower();
                criteria = criteria.And(c => c.Brand.ToLower() == brand);
            }

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                criteria = criteria.And(c =>
                    c.Name.ToLower().Contains(searchTerm) ||
                    c.Brand.ToLower().Contains(searchTerm));
            }

            if (parameters.MinPrice.HasValue)
            {
                criteria = criteria.And(c => c.PricePerDay >= parameters.MinPrice.Value);
            }

            if (parameters.MaxPrice.HasValue)
            {
                criteria = criteria.And(c => c.PricePerDay <= parameters.MaxPrice.Value);
            }

            if (parameters.Available.HasValue)
            {
                criteria = criteria.And(c => c.Available == parameters.Available.Value);
            }

            return criteria;
        }
    }

    // Specification for getting car by ID (not deleted)
    public class CarByIdSpecification : BaseSpecification<Car>
    {
        public CarByIdSpecification(int id)
            : base(c => c.Id == id && !c.IsDeleted)
        {
        }
    }
}
