using Passengers.DataTransferObject.TagDtos;
using Passengers.Models.Main;
using Passengers.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.TagService.Store
{
    public static class TagStore
    {
        public static class Filter
        {
        }

        public static class Query
        {
            public static Expression<Func<Tag, GetTagDto>> GetSelectTag => c => new GetTagDto
            {
                Id = c.Id,
                Name = c.Name,
                LogoPath = c.LogoPath,
                ShopId = c.ShopId
            };

            public static Expression<Func<GetTagDto, Tag>> InverseGetSelectTag => c => new Tag
            {
                Id = c.Id,
                Name = c.Name,
                LogoPath = c.LogoPath,
                ShopId = c.ShopId
            };

            public static Expression<Func<SetTagDto, Tag>> InverseSetSelectCategory => c => new Tag
            {
                Id = c.Id,
                Name = c.Name,
                ShopId = c.ShopId
            };
        }
    }
}
