using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.DriverDtos;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.Models.Security;
using Passengers.Repository.Base;
using Passengers.Security.AccountService;
using Passengers.Security.DriveService.Store;
using Passengers.SharedKernel.Constants;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.Files;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Pagnation;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.DriveService
{
    public class DriverRepository : BaseRepository, IDriverRepository
    {
        private readonly IAccountRepository accountRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<AppUser> userManager;

        public DriverRepository(PassengersDbContext context, IAccountRepository accountRepository, IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager) : base(context)
        {
            this.accountRepository = accountRepository;
            this.webHostEnvironment = webHostEnvironment;
            this.userManager = userManager;
        }

        public async Task<OperationResult<GetDriverDto>> Add(SetDriverDto dto)
        {
            var user = (await accountRepository.Create(new CreateAccountDto
            {
                PhoneNumber = dto.PhoneNumber,
                UserName = "Driver" + Helpers.GetUniqueKey(),
                Type = SharedKernel.Enums.UserTypes.Driver,
                Password = "passengers",
            })).Result;

            if (user != null)
            {
                var entity = await Context.Drivers().Include(x => x.Payments).Where(x => x.Id == user.Id).SingleOrDefaultAsync();

                DriverStore.Query.AssignDtoToDriver(entity, dto);

                if(dto.ImageFile != null)
                {
                    var path = FileExtensions.ProcessUploadedFile(dto.ImageFile, FolderNames.Admin, webHostEnvironment.WebRootPath);
                    if (!path.IsNullOrEmpty())
                    {
                        entity.IdentifierImagePath = path;
                    }
                }

                await Context.SaveChangesAsync();
                return _Operation.SetSuccess(DriverStore.Query.DriverToDriverDto(entity));
            }

            return _Operation.SetFailed<GetDriverDto>("Error");
        }

        public async Task<OperationResult<bool>> Delete(Guid id)
        {
            return await accountRepository.Delete(id);
        }

        public async Task<OperationResult<DetailsDriverDto>> Details(Guid id, DateTime? day)
        {
            var entity = await Context.Drivers().Include(x => x.DriverOrders).Include(x => x.Payments)
                .Where(x => x.Id == id).SingleOrDefaultAsync();
            if (entity == null)
                return _Operation.SetContent<DetailsDriverDto>(OperationResultTypes.NotExist, "");

            var result = DriverStore.Query.DriverToDetailsDriverDto(entity, day);
            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<PagedList<GetDriverDto>>> Get(int pageNumber, int pageSize, string search, bool? online)
        {
            ///ToDO
            pageSize = 100000;
            var users = await Context.Drivers()
                .Include(x => x.DriverOrders).Include(x => x.Payments)
                .Where(x => string.IsNullOrEmpty(search) || x.FullName.Contains(search))
                .OrderByDescending(x => x.DateCreated)
                .ToPagedListAsync(pageNumber, pageSize);
            var result = users.Select(DriverStore.Query.DriverToDriverDto).ToPagedList(pageNumber, pageSize);
            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<GetDriverDto>> GetById(Guid id)
        {
            var entity = await Context.Drivers().Include(x => x.Payments).Where(x => x.Id == id).SingleOrDefaultAsync();
            if (entity == null)
                return _Operation.SetContent<GetDriverDto>(OperationResultTypes.NotExist, "");

            return _Operation.SetSuccess(DriverStore.Query.DriverToDriverDto(entity));
        }

        public async Task<OperationResult<GetDriverDto>> Update(SetDriverDto dto)
        {
            var entity = await Context.Drivers().Include(x => x.Payments).Where(x => x.Id == dto.Id).SingleOrDefaultAsync();
            if (entity == null)
                return _Operation.SetContent<GetDriverDto>(OperationResultTypes.NotExist, "");

            DriverStore.Query.AssignDtoToDriver(entity, dto);

            if(dto.ImageFile != null)
            {
                var path = FileExtensions.ProcessUploadedFile(dto.ImageFile, FolderNames.Admin, webHostEnvironment.WebRootPath);
                if (!path.IsNullOrEmpty())
                {
                    entity.IdentifierImagePath = path;
                }
            }

            await userManager.UpdateAsync(entity);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(DriverStore.Query.DriverToDriverDto(entity));
        }
    }
}
