using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.SharedDtos.CategoryDtos;
using Passengers.Models.Shared;
using Passengers.Repository.Base;
using Passengers.Shared.CategoryService.Store;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.Constants;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.Files;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.CategoryService
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ISharedRepository sharedRepository;

        public CategoryRepository(PassengersDbContext context, IWebHostEnvironment webHostEnvironment, ISharedRepository sharedRepository) : base(context)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.sharedRepository = sharedRepository;
        }
        
        public async Task<OperationResult<GetCategoryDto>> Add(SetCategoryDto dto)
        {
            if(dto.ParentId is not null)
            {
                var isParentExist = await sharedRepository.CheckIsExist<Category>(dto.ParentId.Value);
                if (!isParentExist)
                {
                    return _Operation.SetContent<GetCategoryDto>(OperationResultTypes.NotExist, $"Parent Id {dto.ParentId} not found.");
                }
            }
            var category = CategoryStore.Query.InverseSetSelectCategory.Compile()(dto);
            category.LogoPath = dto.File.TryUploadImage(FolderNames.Category, webHostEnvironment.WebRootPath);
            Context.Categories.Add(category);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(CategoryStore.Query.GetSelectCategory.Compile()(category));
        }
        public async Task<OperationResult<GetCategoryDto>> Update(SetCategoryDto dto)
        {
            if (dto.ParentId is not null)
            {
                var isParentExist = await sharedRepository.CheckIsExist<Category>(dto.ParentId.Value);
                if (!isParentExist)
                {
                    return (OperationResultTypes.NotExist, $"Parent Id {dto.ParentId} not found.");
                }
            }
            var entity = await Context.Categories.Where(CategoryStore.Filter.WhereCategoryId(dto.Id)).SingleOrDefaultAsync();
            if (entity is null)
                return (OperationResultTypes.NotExist, $"Category: {dto.Id} not exist");

            CategoryStore.Query.AssignDtoToCategory(entity, dto);
            if (dto.File is not null)
            {
                entity.LogoPath = dto.File.TryUploadImage(FolderNames.Category, webHostEnvironment.WebRootPath);
            }

            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(CategoryStore.Query.GetSelectCategory.Compile()(entity));
        }
        public async Task<OperationResult<bool>> Remove(Guid id)
        {
            var category = await Context.Categories.Where(CategoryStore.Filter.WhereCategoryId(id)).SingleOrDefaultAsync();
            if (category == null)
                return _Operation.SetFailed<bool>("Category Not Found.", OperationResultTypes.NotExist);
            await RemoveAllChildren(category);
            return _Operation.SetSuccess(true);
        }
        
        public async Task<OperationResult<List<GetCategoryDto>>> Get()
        {
            var categories = await Context.Categories
                .Select(CategoryStore.Query.GetSelectCategory)
                .ToListAsync();
            return _Operation.SetSuccess(categories);
        }
        public async Task<OperationResult<GetCategoryDto>> GetById(Guid id)
        {
            var entity = await Context.Categories.Where(CategoryStore.Filter.WhereCategoryId(id)).SingleOrDefaultAsync();
            if (entity is null)
                return (OperationResultTypes.NotExist, $"Category: {id} not exist");
            return _Operation.SetSuccess(CategoryStore.Query.GetSelectCategory.Compile()(entity));
        }
        public async Task<OperationResult<List<GetCategoryDto>>> GetChildern(Guid id)
        {
            var categories = await Context.Categories
                .Include(x => x.Categories)
                .Where(CategoryStore.Filter.WhereParentId(id))
                .Select(CategoryStore.Query.GetSelectCategory)
                .ToListAsync();
            return _Operation.SetSuccess(categories);
        }
        public async Task<OperationResult<List<GetCategoryDto>>> GetRoots()
        {
            var categories = await Context.Categories
                .Include(x => x.Categories)
                .Where(CategoryStore.Filter.WhereRootId)
                .Select(CategoryStore.Query.GetSelectCategory)
                .ToListAsync();
            return _Operation.SetSuccess(categories);
        }
        public async Task<OperationResult<TreeDto>> GetTree(Guid id)
        {
            var categories = await GetTreeWithSingleRoot(id, -1);
            return _Operation.SetSuccess(categories);
        }
        public async Task<OperationResult<List<string>>> GetFullPath(Guid id)
        {
            var category = await Context.Categories.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (category is null)
            {
                return (OperationResultTypes.NotExist, $"Category: {id} not exist");
            }
            await GetPath(id);
            levels.Reverse();
            return _Operation.SetSuccess(levels);
        }

        #region -Function Helper-
        private async Task<TreeDto> GetTreeWithSingleRoot(Guid id, int level)
        {
            TreeDto tree = new();
            var parent = await Context.Categories
                .Where(CategoryStore.Filter.WhereCategoryId(id))
                .Include(c => c.Parent)
                .Include(c => c.Categories)
                .Select(CategoryStore.Query.GetSelectCategory)
                .SingleOrDefaultAsync();
            tree.Id = parent.Id;
            tree.Name = parent.Name;
            tree.ParentId = parent.ParentId;
            tree.Path = parent.Path;
            tree.Children = new();

            var children = await Context.Categories
                .Where(CategoryStore.Filter.WhereParentId(id))
                .Include(c => c.Parent)
                .Include(c => c.Categories)
                .ToListAsync();

            if (level != 0)
            {
                foreach (var child in children)
                {
                    tree.Children.Add(await GetTreeWithSingleRoot(child.Id, level - 1));
                }
            }
            return tree;
        }

        List<string> levels = new();
        private async Task GetPath(Guid id)
        {
            var category = await Context.Categories
                .Include(x => x.Parent)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            levels.Add(category.Name);
            if(category.Parent != null)
            {
                await GetPath(category.Parent.Id);
            }
            return;
        }

        private async Task<bool> RemoveAllChildren(Category category)
        {
            var categories = await Context.Categories.Where(CategoryStore.Filter.WhereParentId(category.Id)).ToListAsync();
            foreach (var cat in categories)
            {
                await RemoveAllChildren(cat);
            }
            category.DateDeleted = DateTime.Now;
            category.LogoPath.TryDeleteImage(webHostEnvironment.WebRootPath);
            await Context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetByShopId(Guid id)
        {
            var shop = await Context.Shops().Where(x => x.Id == id).Include(x => x.Category).SingleOrDefaultAsync();
            if (shop is null)
                return null;
            var name = shop.Category?.Name;
            return name;
        }
        #endregion
    }
}
