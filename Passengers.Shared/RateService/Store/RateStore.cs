using Passengers.DataTransferObject.RateDtos;
using Passengers.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.RateService.Store
{
    public static class RateStore
    {
        public static class Query
        {
            public static Expression<Func<Review, RateDto>> GetSelectCategory => c => new RateDto
            {
                Id = c.Id,
                
            };

            public static Expression<Func<RateDto, Category>> InverseGetSelectCategory => c => new Category
            {
                Id = c.Id,
            };

        }
    }
}
