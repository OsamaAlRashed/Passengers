using Passengers.DataTransferObject.ProductDtos;
using Passengers.Models.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.ProductService.Store
{
    public static class ProducStore
    {
        public static class Query
        {
            public static Expression<Func<Product, GetProductDto>> GetSelectTag => c => new GetProductDto
            {
                Id = c.Id,
                Name = c.Name,
                Avilable = c.Avilable,
                Description = c.Description,
                ImagePath = c.Documents.Select(x => x.Path).FirstOrDefault(),
                PrepareTime = c.PrepareTime,
                Price = c.Price,
                TagId = c.TagId,
            };
        }
    }
}
