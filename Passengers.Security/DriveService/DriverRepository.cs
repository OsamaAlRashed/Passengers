using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.DriverDtos;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.Models.Location;
using Passengers.Models.Security;
using Passengers.Repository.Base;
using Passengers.Security.AccountService;
using Passengers.Security.DriveService.Store;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.Constants;
using Passengers.SharedKernel.Enums;
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
                var entity = await Context.Drivers().Include(x => x.Address).Include(x => x.Payments).Where(x => x.Id == user.Id).SingleOrDefaultAsync();

                DriverStore.Query.AssignDtoToDriver(entity, dto);

                if(dto.ImageFile != null)
                {
                    var path = FileExtensions.ProcessUploadedFile(dto.ImageFile, FolderNames.Admin, webHostEnvironment.WebRootPath);
                    if (!path.IsNullOrEmpty())
                    {
                        entity.IdentifierImagePath = path;
                    }
                }

                entity.Address = new Address()
                {
                    Text = dto.AddressText,
                    AreaId = Context.Areas.FirstOrDefault().Id
                };
                Context.Addresses.Add(entity.Address);

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
            var entity = await Context.Drivers().Include(x => x.Address).Include(x => x.DriverOrders).Include(x => x.Payments)
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
                .Include(x => x.Address)
                .Include(x => x.DriverOrders).Include(x => x.Payments)
                .Where(x => string.IsNullOrEmpty(search) || x.FullName.Contains(search))
                .OrderByDescending(x => x.DateCreated)
                .ToPagedListAsync(pageNumber, pageSize);
            var result = users.Select(DriverStore.Query.DriverToDriverDto).ToPagedList(pageNumber, pageSize);
            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<GetDriverDto>> GetById(Guid id)
        {
            var entity = await Context.Drivers().Include(x => x.Address).Include(x => x.Payments).Where(x => x.Id == id).SingleOrDefaultAsync();
            if (entity == null)
                return _Operation.SetContent<GetDriverDto>(OperationResultTypes.NotExist, "");

            return _Operation.SetSuccess(DriverStore.Query.DriverToDriverDto(entity));
        }

        public async Task<OperationResult<GetDriverDto>> Update(SetDriverDto dto)
        {
            var entity = await Context.Drivers().Include(x => x.Address).Include(x => x.Payments).Where(x => x.Id == dto.Id).SingleOrDefaultAsync();
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

            if(entity.Address == null)
            {
                entity.Address = new Address()
                {
                    Text = dto.AddressText,
                    AreaId = Context.Areas.FirstOrDefault().Id
                };
                Context.Addresses.Add(entity.Address);
            }
            else
            {
                entity.Address.Text = dto.AddressText;
            }

            await userManager.UpdateAsync(entity);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(DriverStore.Query.DriverToDriverDto(entity));
        }

        public async Task<OperationResult<bool>> ChangeAvilability(bool status, string lat , string lng)
        {
            var currentUser = await Context.Drivers()
                .Include(x => x.Address).SingleOrDefaultAsync(x => x.Id == Context.CurrentUserId);
            if (currentUser == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "");

            currentUser.DriverOnline = status;
            if (status)
            {
                if(currentUser.Address == null)
                {
                    currentUser.Address = new Address();
                    currentUser.Address.AreaId = Context.Areas.FirstOrDefault().Id;
                    Context.Addresses.Add(currentUser.Address);
                }
                currentUser.Address.Lat = lat;
                currentUser.Address.Long = lng;
            }

            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<object>> Login(LoginDriverDto dto)
        {
            var result = await accountRepository.Login(DriverStore.Query.DriverToBaseLoginDto(dto));
            if (result.IsSuccess)
            {
                var user = result.Result.User;
                var response = new
                {
                    user.Id,
                    user.UserName,
                    user.PhoneNumber,
                    result.Result.AccessToken,
                    result.Result.RefreshToken,
                    user.AccountStatus,
                    user.FullName,
                    user.GenderType,
                    user.DOB,
                    user.BloodType,
                    ImagePath = user.IdentifierImagePath
                };
                return _Operation.SetSuccess<object>(response);
            }
            return _Operation.SetFailed<object>(result.Message, result.OperationResultType);
        }

        public async Task<OperationResult<string>> UploadImage(IFormFile file)
        {
            var customer = await Context.Drivers().Where(x => x.Id == Context.CurrentUserId)
                .SingleOrDefaultAsync();
            if (customer == null)
                return _Operation.SetContent<string>(OperationResultTypes.NotExist, "");

            var imagePath = file.TryUploadImage("Drivers", webHostEnvironment.WebRootPath);
            if (imagePath.IsNullOrEmpty())
                return _Operation.SetFailed<string>("UploadImageFailed");

            customer.IdentifierImagePath = imagePath;
            await Context.SaveChangesAsync();

            return imagePath;
        }

        public async Task<OperationResult<GetDriverDto>> GetMyInformations() 
            => await GetById(Context.CurrentUserId.Value);

        public async Task<OperationResult<DriverStatisticsDto>> GetStatistics(DateTime? date)
        {
            var driver = await Context.Drivers().Include(x => x.Payments).Include(x => x.DriverOrders).ThenInclude(x => x.OrderStatusLogs)
                .Where(x => x.Id == Context.CurrentUserId).SingleOrDefaultAsync();

            if (driver == null)
                return _Operation.SetContent<DriverStatisticsDto>(OperationResultTypes.NotExist,"User not exist.");

            DriverStatisticsDto dto = new();
            dto.FixedAmount = Math.Abs(driver.FixedAmount());
            dto.DeliveryAmount = Math.Abs(driver.DeliveryAmount());
            dto.OrderCount = driver.DriverOrders.Count();
            var deliveries = driver.DriverOrders.Where(x => (!date.HasValue || x.DateCreated == date) && x.Status() == OrderStatus.Completed)
                .Select(x => x.OrderStatusLogs.Where(x => x.Status == OrderStatus.Completed).Select(x => x.DateCreated).FirstOrDefault()
                .Subtract(x.OrderStatusLogs.Where(x => x.Status == OrderStatus.Assigned).Select(x => x.DateCreated).FirstOrDefault()))
                .ToList();

            dto.SpeedAverage = deliveries.Any() ? Math.Round(deliveries.Average(x => x.TotalMinutes), 0) : null;

            return _Operation.SetSuccess(dto);
        }
    }
}
