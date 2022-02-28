using Passengers.DataTransferObject.SharedDtos;
using Passengers.DataTransferObject.SharedDtos.CategoryDtos;
using Passengers.Models.Shared;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.CategoryService.Store
{
    public static class CategoryStore
    {
        public static class Filter
        {
            public static Expression<Func<Category, bool>> WhereCategoryId(Guid id) => (category) => category.Id == id;
            public static Expression<Func<Category, bool>> WhereParentId(Guid id) => (category) => category.ParentId == id;
            public static Expression<Func<Category, bool>> WhereRootId => (category) => category.ParentId == null;
        }

        public static class Query
        {
            public static Expression<Func<Category, GetCategoryDto>> GetSelectCategory => c => new GetCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Path = c.LogoPath,
                ParentId = c.ParentId
            };

            public static Expression<Func<GetCategoryDto, Category>> InverseGetSelectCategory => c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                LogoPath = c.Path,
                ParentId = c.ParentId
            };

            public static Expression<Func<SetCategoryDto, Category>> InverseSetSelectCategory => c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentId
            };
        }
    }
}
