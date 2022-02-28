using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Passengers.DataTransferObject.TagDtos;
using Passengers.Main.TagService.Store;
using Passengers.Models.Main;
using Passengers.Repository.Base;
using Passengers.Shared.CategoryService.Store;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.Constants;
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

namespace Passengers.Main.TagService
{
    public class TagRepository : BaseRepository, ITagRepository
    {
        private readonly ISharedRepository sharedRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public TagRepository(PassengersDbContext context, ISharedRepository sharedRepository, IWebHostEnvironment webHostEnvironment) : base(context)
        {
            this.sharedRepository = sharedRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<OperationResult<GetTagDto>> Add(SetTagDto dto)
        {
            var tag = TagStore.Query.InverseSetSelectCategory.Compile()(dto);
            tag.LogoPath = dto.LogoFile.TryUploadImage(FolderNames.Tags, webHostEnvironment.WebRootPath);
            Context.Tags.Add(tag);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(TagStore.Query.GetSelectTag.Compile()(tag));
        }

        public async Task<OperationResult<List<GetTagDto>>> Get()
        {
            var tags = await Context.Tags
                .Select(TagStore.Query.GetSelectTag)
                .ToListAsync();
            return _Operation.SetSuccess(tags);
        }

        public async Task<OperationResult<GetTagDto>> GetById(Guid id)
        {
            var tag = await Context.Tags
                .Where(x => x.Id == id)
                .Select(TagStore.Query.GetSelectTag)
                .SingleOrDefaultAsync();
            return _Operation.SetSuccess(tag);
        }

        public async Task<OperationResult<List<GetTagDto>>> GetByShopId(Guid id)
        {
            var tags = await Context.Tags
                .Where(x => x.ShopId == id)
                .Select(TagStore.Query.GetSelectTag)
                .ToListAsync();
            return _Operation.SetSuccess(tags);
        }

        public async Task<OperationResult<List<GetTagDto>>> GetPublicTag()
        {
            var tags = await Context.Tags
                .Where(x => !x.ShopId.HasValue)
                .Select(TagStore.Query.GetSelectTag)
                .ToListAsync();
            return _Operation.SetSuccess(tags);
        }

        public async Task<OperationResult<bool>> Remove(Guid id)
        {
            var tag = await Context.Tags.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (tag == null)
                return _Operation.SetFailed<bool>("Category Not Found.", OperationResultTypes.NotExist);
            //await RemoveProducts(Guid id);
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<GetTagDto>> Update(SetTagDto dto)
        {
            var entity = await Context.Tags.Where(x => x.Id == dto.Id).SingleOrDefaultAsync();
            if (entity == null)
                return _Operation.SetFailed<GetTagDto>("TagNotFound.", OperationResultTypes.NotExist);

            if (dto.LogoFile is not null)
            {
                entity.LogoPath = dto.LogoFile.TryUploadImage(FolderNames.Category, webHostEnvironment.WebRootPath);
            }
            entity.Name = dto.Name;
            entity.ShopId = dto.ShopId;

            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(TagStore.Query.GetSelectTag.Compile()(entity));
        }
    }
}
