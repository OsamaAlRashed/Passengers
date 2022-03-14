using Passengers.DataTransferObject.DiscountDtos;
using Passengers.Models.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.DiscountService.Store
{
    public static class DiscountStore
    {
        public static class Query
        {
            public static Expression<Func<Discount, DiscountDto>> GetSelectDiscount => c => new DiscountDto
            {
                Id = c.Id,
                Price = c.Price,
                ProductId = c.ProductId,
                StartDate = c.StartDate,
                EndDate = c.EndDate
            };

            public static Expression<Func<DiscountDto, Discount>> SetSelectDiscount => c => new Discount
            {
                Id = c.Id,
                Price = c.Price,
                ProductId = c.ProductId,
                StartDate = c.StartDate,
                EndDate = c.EndDate
            };

        }

        
    }
}
