using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Passengers.DataTransferObject.TagDtos;
using Passengers.Main.TagService.Store;
using Passengers.Models.Main;
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

namespace Passengers.Main.TagService
{
    public class TagRepository : BaseRepository, ITagRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public TagRepository(PassengersDbContext context, IWebHostEnvironment webHostEnvironment) : base(context)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<OperationResult<GetTagDto>> Add(SetTagDto dto)
        {
            var user = await Context.Users.Where(x => x.Id == Context.CurrentUserId).SingleOrDefaultAsync();
            if(user == null)
                return _Operation.SetContent<GetTagDto>(OperationResultTypes.NotExist, "");

            var tag = TagStore.Query.InverseSetSelectCategory.Compile()(dto);

            tag.ShopId = user.UserType == UserType.Shop ? Context.CurrentUserId : null;
            tag.LogoPath = dto.LogoFile.TryUploadImage(FolderNames.Tag, webHostEnvironment.WebRootPath);
            Context.Tags.Add(tag);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(TagStore.Query.GetSelectTag.Compile()(tag));
        }

        public async Task<OperationResult<bool>> ChangeStatus(Guid id, bool status)
        {
            var tag = await Context.Tags.Include(x => x.Products)
                .Where(x => x.Id == id).SingleOrDefaultAsync();
            if (tag == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "");

            tag.IsHidden = status;
            foreach (var product in tag.Products)
            {
                product.IsHidden = status;
            }
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<List<GetTagDto>>> Get(bool isHidden)
        {
            var tags = await Context.Tags
                .Where(x => x.IsHidden == isHidden)
                .Select(TagStore.Query.GetSelectTag)
                .ToListAsync();
            return _Operation.SetSuccess(tags);
        }

        public async Task<OperationResult<GetTagDto>> GetById(Guid id)
        {
            var tag = await Context.Tags
                .Where(x => x.Id == id).Select(TagStore.Query.GetSelectTag)
                .SingleOrDefaultAsync();
            if (tag == null)
                return _Operation.SetFailed<GetTagDto>("", OperationResultTypes.NotExist);

            return _Operation.SetSuccess(tag);
        }

        public async Task<OperationResult<List<GetTagDto>>> GetByShopId(Guid? id)
        {
            var tags = await Context.Tags
                .Where(x => !id.HasValue ? x.ShopId == Context.CurrentUserId : x.ShopId == id)
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
                return _Operation.SetFailed<bool>("Tag Not Found.", OperationResultTypes.NotExist);

            tag.DateDeleted = DateTime.UtcNow;
            await Context.SaveChangesAsync();
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

        public async Task<OperationResult<bool>> DeleteDraftTags(Guid shopId)
        {
            var tags = await Context.Tags.Where(x => x.ShopId == shopId).ToListAsync();
            if (tags == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "UserNotFound");

            Context.Tags.RemoveRange(tags);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }
    }
}
