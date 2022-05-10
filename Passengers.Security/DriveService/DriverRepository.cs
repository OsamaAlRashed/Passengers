using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.DriverDtos;
using Passengers.DataTransferObject.SecurityDtos;
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

        public DriverRepository(PassengersDbContext context, IAccountRepository accountRepository, IWebHostEnvironment webHostEnvironment): base(context)
        {
            this.accountRepository = accountRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<OperationResult<GetDriverDto>> Add(SetDriverDto dto)
        {
            var user = (await accountRepository.Create(new CreateAccountDto
            {
                PhoneNumber = dto.PhoneNumber,
                UserName = "Passenger" + Helpers.GetUniqueKey(),
                Type = SharedKernel.Enums.UserTypes.Driver,
                Password = Helpers.GetUniqueKey(4),
            })).Result;

            if (user != null)
            {
                var entity = await Context.Drivers().Where(x => x.Id == user.Id).SingleOrDefaultAsync();

                DriverStore.Query.AssignDtoToDriver(entity, dto);

                var path = FileExtensions.ProcessUploadedFile(dto.ImageFile, FolderNames.Admin, webHostEnvironment.WebRootPath);
                if (!path.IsNullOrEmpty())
                {
                    entity.IdentifierImagePath = path;
                }

                await Context.SaveChangesAsync();
                return _Operation.SetSuccess(DriverStore.Query.DriverToDriverDto(entity));
            }

            return _Operation.SetFailed<GetDriverDto>("Error");
        }

        public async Task<OperationResult<bool>> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<DetailsDriverDto>> Details(Guid id, DateTime? day)
        {
            var entity = await Context.Drivers().Include(x => x.DriverOrders).Where(x => x.Id == id).SingleOrDefaultAsync();
            if (entity == null)
                return _Operation.SetContent<DetailsDriverDto>(OperationResultTypes.NotExist, "");

            var orders = entity.DriverOrders;
            if (day != null)
            {
                orders = orders.Where(x => x.DateCreated == day).ToList();
            }

            var result = DriverStore.Query.DriverToDetailsDriverDto(entity);
            //result.DeliveryAmount = orders.Sum(x => x.DeliveryCost);
            result.OrderCount = orders.Count();
            //result.OnlineTime = 
            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<PagedList<GetDriverDto>>> Get(int pageNumber, int pageSize, string search)
        {
            var users = await Context.Drivers().Where(x => string.IsNullOrEmpty(search) || x.FullName.Contains(search)).ToPagedListAsync(pageNumber, pageSize);
            var result = users.Select(DriverStore.Query.DriverToDriverDto).ToPagedList(pageNumber, pageSize);
            return _Operation.SetSuccess(result);
        }

        public Task<OperationResult<GetDriverDto>> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<GetDriverDto>> Update(SetDriverDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
