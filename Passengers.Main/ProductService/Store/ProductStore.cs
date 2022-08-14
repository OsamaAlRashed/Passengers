using Passengers.DataTransferObject.ProductDtos;
using Passengers.Models.Main;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.ProductService.Store
{
    public static class ProductStore
    {
        public static class Filter
        {
            public static Expression<Func<Product, bool>> WhereFilter(ProductFilterDto filter) => product =>
                (!filter.Rate.HasValue || (int)(product.Reviews.Any() ? product.Reviews.Average(x => x.Rate) : 0) == filter.Rate)
             && (!filter.FromPrice.HasValue || filter.FromPrice <= product.Price())
             && (!filter.ToPrice.HasValue || filter.ToPrice >= product.Price())
             && ((!filter.Discount.HasValue) ||
                (filter.Discount.Value ? product.Discounts.Where(x => x.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= x.EndDate).Any()
                                       : !product.Discounts.Where(x => x.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= x.EndDate).Any()))
             && (filter.TagIds == null || !filter.TagIds.Any() || filter.TagIds.Contains(product.TagId))
             && (string.IsNullOrEmpty(filter.Search) || product.Name.Contains(filter.Search))
             && (!filter.Avilable.HasValue || product.Avilable == filter.Avilable);
        }
        public static class Query
        {
            public static Expression<Func<Product, GetProductDto>> GetSelectProduct => c => new GetProductDto
            {
                Id = c.Id,
                Name = c.Name,
                Avilable = c.Avilable,
                Description = c.Description,
                ImagePath = c.ImagePath(),
                PrepareTime = c.PrepareTime,
                Price = c.Price(),
                TagId = c.TagId,
                HasDiscount = c.Discounts.Any(x => x.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= x.EndDate),
                Discount = c.Discounts.Any(x => x.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= x.EndDate) ? c.Discounts.FirstOrDefault(x => x.StartDate <= DateTime.UtcNow && DateTime.UtcNow <= x.EndDate).Price : null,
                IsNew = DateTime.UtcNow.Day - c.DateCreated.Day <= 2,
                Rate = c.Reviews.Any() ? c.Reviews.Average(x => x.Rate) : 0,
            };

            public static Expression<Func<Product, object>> Sort(SortProductTypes? sortType)
            {
                if (sortType == null)
                    return x => x.DateCreated;
                return sortType switch
                {
                    SortProductTypes.Name => x => x.Name,
                    SortProductTypes.Rate => x => x.Reviews.Any() ? x.Reviews.Average(x => x.Rate) : 0,
                    SortProductTypes.PrepareTime => x => x.PrepareTime,
                    SortProductTypes.Avilable => x => x.Avilable,
                    SortProductTypes.Price => x => x.Price(),
                    _ => x => x.DateCreated,
                };
            }
        }
    }
}
