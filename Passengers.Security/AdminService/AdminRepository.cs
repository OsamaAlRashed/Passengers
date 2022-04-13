﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.Location.AddressSerive;
using Passengers.Models.Location;
using Passengers.Models.Security;
using Passengers.Repository.Base;
using Passengers.Security.AccountService;
using Passengers.Security.AdminService.Store;
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

namespace Passengers.Security.AdminService
{
    public class AdminRepository : BaseRepository, IAdminRepository
    {
        private readonly IAccountRepository accountRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<AppUser> userManager;

        public AdminRepository(PassengersDbContext context, IAccountRepository accountRepository, IWebHostEnvironment webHostEnvironment
            , UserManager<AppUser> userManager) : base(context)
        {
            this.accountRepository = accountRepository;
            this.webHostEnvironment = webHostEnvironment;
            this.userManager = userManager;
        }

        public async Task<OperationResult<GetAdminDto>> Add(SetAdminDto dto)
        {
            var user = (await accountRepository.Create(new CreateAccountDto
            {
                PhoneNumber = dto.PhoneNumber,
                UserName = dto.UserName,
                Type = SharedKernel.Enums.UserTypes.Admin,
                Password = dto.Password,
            })).Result;

            if(user != null)
            {
                var entity = await Context.Admins().Where(x => x.Id == user.Id).SingleOrDefaultAsync();

                AdminStore.Query.AssignDtoToAdmin(entity, dto);
                
                var path = FileExtensions.ProcessUploadedFile(dto.ImageFile, FolderNames.Admin, webHostEnvironment.WebRootPath);
                if (!path.IsNullOrEmpty())
                {
                    entity.IdentifierImagePath = path;
                }

                await Context.SaveChangesAsync();
                return _Operation.SetSuccess(AdminStore.Query.AdminToAdminDto(entity));
            }

            return _Operation.SetFailed<GetAdminDto>("Error");
        }

        public async Task<OperationResult<bool>> Block(Guid id)
        {
            var user = await Context.Users.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (user == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "UserNotFound");

            user.DateBlocked = user.DateBlocked.HasValue ? null : DateTime.Now;
            await Context.SaveChangesAsync();

            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<GetAdminDto>> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<PagedList<GetAdminDto>>> Get(int pageNumber, int pageSize, string search)
        {
            var users = await Context.Admins().Where(x => string.IsNullOrEmpty(search) || x.FullName.Contains(search)).ToPagedListAsync(pageNumber, pageSize);
            var result = users.Select(AdminStore.Query.AdminToAdminDto).ToPagedList(pageNumber, pageSize);
            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<GetAdminDto>> GetById(Guid id)
        {
            var entity = await Context.Admins().Where(x => x.Id == id).SingleOrDefaultAsync();
            if (entity == null)
                return _Operation.SetContent<GetAdminDto>(OperationResultTypes.NotExist, "");

            return _Operation.SetSuccess(AdminStore.Query.AdminToAdminDto(entity));
        }

        public async Task<OperationResult<PagedList<DashboardShopDto>>> GetShops(int pageNumber, int pageSize, string search)
        {
            var shops = await Context.Shops()
                .Include(x => x.MainCategories).ThenInclude(x => x.Category)
                .Include(x => x.Documents).Include(x => x.ShopSchedules)
                .Include(x => x.Tags).ThenInclude(x => x.Products).ThenInclude(x => x.Rates)
                .Where(x => x.Name.Contains(search) || x.MainCategories.Select(x => x.Category.Name).Select(cat => cat.Contains(search)).Any())
                .OrderByDescending(x => x.DateCreated)
                .Select(x => new DashboardShopDto
                {
                    Id = x.Id,
                    Name= x.Name,
                    PhoneNumber = x.PhoneNumber,
                    ImagePath = x.Documents.Select(x => x.Path).FirstOrDefault(),
                    Category = x.MainCategories.Select(x => x.Category.Name).FirstOrDefault(),
                    Online = x.ShopSchedules.Any(x => x.Days.Contains(DateTime.Now.Day.ToString()) 
                                                   && x.FromTime <= DateTime.Now.TimeOfDay 
                                                   && x.ToTime >= DateTime.Now.TimeOfDay),
                    FromDay = x.ShopSchedules.Any() ? 
                        (x.ShopSchedules.FirstOrDefault().Days.IsNullOrEmpty() ?
                            null : (x.ShopSchedules.FirstOrDefault().Days[0] - 49))
                                : null,
                    ToDay = x.ShopSchedules.Any() ?
                        (x.ShopSchedules.FirstOrDefault().Days.IsNullOrEmpty() ?
                            null : (int)(x.ShopSchedules.FirstOrDefault().Days[x.ShopSchedules.FirstOrDefault().Days.Length - 1] - 49))
                                : null,
                    FromTime = x.ShopSchedules.FirstOrDefault().FromTime.ToString(@"hh\:mm"),
                    ToTime = x.ShopSchedules.FirstOrDefault().ToTime.ToString(@"hh\:mm"),
                    Contacts = x.ShopContacts.Select(xx => new
                    {
                        xx.Type,
                        xx.Text,
                    }).ToList(),
                    Address = new
                    {
                        Lat = x.Address.Lat,
                        Long = x.Address.Long,
                        Text = x.Address.Text
                    },
                    DeliveryShopStatus = x.DeliveryShopStatus
                }).ToPagedListAsync(pageNumber, pageSize);

            return _Operation.SetSuccess(shops);
        }

        public async Task<OperationResult<GetAdminDto>> Update(SetAdminDto dto)
        {
            var entity = await Context.Admins().Where(x => x.Id == dto.Id).SingleOrDefaultAsync();
            if (entity == null)
                return _Operation.SetContent<GetAdminDto>(OperationResultTypes.NotExist, "");

            AdminStore.Query.AssignDtoToAdmin(entity, dto);

            var path = FileExtensions.ProcessUploadedFile(dto.ImageFile, FolderNames.Admin, webHostEnvironment.WebRootPath);
            if (!path.IsNullOrEmpty())
            {
                entity.IdentifierImagePath = path;
            }

            if (!dto.Password.IsNullOrEmpty())
            {
                await accountRepository.ChangePassword(entity.Id, dto.Password);
            }

            await userManager.UpdateAsync(entity);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(AdminStore.Query.AdminToAdminDto(entity));
        }

    }
}