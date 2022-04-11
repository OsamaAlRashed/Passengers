﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.CustomerDtos;
using Passengers.DataTransferObject.ProductDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.Models.Security;
using Passengers.Models.Shared;
using Passengers.Repository.Base;
using Passengers.Security.AccountService;
using Passengers.Security.CustomerService.Store;
using Passengers.Security.Shared.Store;
using Passengers.Shared.DocumentService;
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

namespace Passengers.Security.CustomerService
{
    public class CustomerRepository : BaseRepository, ICustomerRepository
    {
        private readonly IAccountRepository accountRepository;
        private readonly UserManager<AppUser> userManager;
        private readonly IDocumentRepository documentRepository;
        private readonly IWebHostEnvironment webHost;

        public CustomerRepository(PassengersDbContext context, IAccountRepository accountRepository, UserManager<AppUser> userManager, IDocumentRepository documentRepository, IWebHostEnvironment webHost): base(context)
        {
            this.accountRepository = accountRepository;
            this.userManager = userManager;
            this.documentRepository = documentRepository;
            this.webHost = webHost;
        }

        public async Task<OperationResult<CustomerInformationDto>> Details()
        {
            var customer = await Context.Customers().Where(x => x.Id == Context.CurrentUserId)
                .SingleOrDefaultAsync();
            if (customer == null)
                return _Operation.SetContent<CustomerInformationDto>(OperationResultTypes.NotExist, "");

            return _Operation.SetSuccess(new CustomerInformationDto
            {
                DOB = customer.DOB.Value,
                FullName = customer.FullName,
                GenderType = customer.GenderType.Value
            });
        }

        public async Task<OperationResult<bool>> Favorite(Guid productId)
        {
            var fav = new Favorite
            {
                ProductId = productId,
                CustomerId = Context.CurrentUserId.Value
            };

            Context.Favorites.Add(fav);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> Follow(Guid shopId)
        {
            var fav = new Favorite
            {
                ShopId = shopId,
                CustomerId = Context.CurrentUserId.Value
            };

            Context.Favorites.Add(fav);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<PagedList<GetProductDto>>> GetMyFavorite(int pageNumber = 1, int pageSize = 10)
        {
            var products = await Context.Products
                .Include(x => x.Documents).Include(x => x.OrderDetails).Include(x => x.Rates).Include(x => x.Discounts)
                .Where(x => x.Favorites.Select(x => x.CustomerId).Contains(Context.CurrentUserId.Value))
                .Select(CustomerStore.Query.GetSelectProduct)
                .ToPagedListAsync(pageNumber, pageSize);

            return _Operation.SetSuccess(products);
        }

        public async Task<OperationResult<PagedList<GetProductDto>>> GetProducts(CustomerProductFilterDto filterDto, int pageNumber = 1, int pageSize = 10)
        {
            var products = await Context.Products
                .Include(x => x.Documents).Include(x => x.OrderDetails).Include(x => x.Rates).Include(x => x.Discounts)
                .Where(CustomerStore.Filter.WhereFilterProduct(filterDto))
                .Select(CustomerStore.Query.GetSelectProduct)
                .ToPagedListAsync(pageNumber, pageSize);

            return _Operation.SetSuccess(products);
        }

        public async Task<OperationResult<PagedList<ShopCustomerDto>>> GetShops(CustomerShopFilterDto filterDto, bool? topShops, int pageNumber = 1, int pageSize = 10)
        {
            List<Guid> shopIds = new();
            if (filterDto.Days != null && filterDto.Days.Any())
            {
                string days = filterDto.Days.Select(x => x.ToString()).Aggregate((i,j) => i + j);
                shopIds = (await Context.ShopSchedules.ToListAsync()).Where(x => x.Days.Intersect(days).Any()).Select(x => x.ShopId).ToList();
            }

            var shops = await Context.Shops()
                .Include(x => x.MainCategories).ThenInclude(x => x.Category)
                .Include(x => x.Documents).Include(x => x.ShopSchedules)
                .Include(x => x.Tags).ThenInclude(x => x.Products).ThenInclude(x => x.Rates)
                .Where(CustomerStore.Filter.WhereFilterShop(filterDto, shopIds))
                .SortBy(CustomerStore.Query.SortShop(topShops),true)
                .Select(CustomerStore.Query.ShopToShopCustomerDto)
                .ToPagedListAsync(pageNumber, pageSize);

            return _Operation.SetSuccess(shops);
        }

        public async Task<OperationResult<object>> Login(LoginCustomerDto dto)
        {
            var result = await accountRepository.Login(CustomerStore.Query.CustomerToBaseLoginDto(dto));
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
                    ImagePath = user.IdentifierImagePath
                };
                return _Operation.SetSuccess<object>(response);
            }
            return _Operation.SetFailed<object>(result.Message, result.OperationResultType);
        }

        public async Task<OperationResult<CreateAccountCustomerDto>> SignUp(CreateAccountCustomerDto dto)
        {
            if (await accountRepository.IsPhoneNumberUsed(dto.PhoneNumber))
                return _Operation.SetFailed<CreateAccountCustomerDto>("PhoneNumberUsed");

            AppUser customer = new()
            {
                PhoneNumber = dto.PhoneNumber,
                UserName = "Customer" + dto.PhoneNumber,
                Email = "Customer" + dto.PhoneNumber + "@Customer",
                FullName = dto.FullName,
                UserType = UserTypes.Customer,
                AccountStatus = AccountStatus.Accepted,
                GenderType = dto.Gender,
                DOB = dto.DOB,
            };
            var identityResult = await userManager.CreateAsync(customer, dto.Password);

            if (!identityResult.Succeeded)
                return _Operation.SetFailed<CreateAccountCustomerDto>(String.Join(",", identityResult.Errors.Select(error => error.Description)));

            var roleIdentityResult = await userManager.AddToRoleAsync(customer, UserTypes.Customer.ToString());

            if (!roleIdentityResult.Succeeded)
                return _Operation.SetFailed<CreateAccountCustomerDto>(String.Join(",", roleIdentityResult.Errors.Select(error => error.Description)));

            await Context.SaveChangesAsync();
            dto.Id = customer.Id;
            return _Operation.SetSuccess(dto);
        }

        public async Task<OperationResult<bool>> UnFavorite(Guid productId)
        {
            var fav = await Context.Favorites.Where(x => x.CustomerId == Context.CurrentUserId && x.ProductId == productId).FirstOrDefaultAsync();
            if (fav is null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "");
            
            Context.Favorites.Remove(fav);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> UnFollow(Guid shopId)
        {
            var fav = await Context.Favorites.Where(x => x.CustomerId == Context.CurrentUserId && x.ShopId == shopId).FirstOrDefaultAsync();
            if (fav is null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "");

            Context.Favorites.Remove(fav);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> UpdateInformation(CustomerInformationDto dto)
        {
            var customer = await Context.Customers().Where(x => x.Id == Context.CurrentUserId)
                .SingleOrDefaultAsync();
            if (customer == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "");

            customer.GenderType = dto.GenderType;
            customer.FullName = dto.FullName;
            customer.DOB = dto.DOB;
            await Context.SaveChangesAsync();

            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<string>> UploadImage(IFormFile file)
        {
            var customer = await Context.Customers().Where(x => x.Id == Context.CurrentUserId)
                .SingleOrDefaultAsync();
            if (customer == null)
                return _Operation.SetContent<string>(OperationResultTypes.NotExist, "");

            var imagePath = file.TryUploadImage("Customers", webHost.WebRootPath);
            if(imagePath.IsNullOrEmpty())
                return _Operation.SetFailed<string>("UploadImageFailed");

            customer.IdentifierImagePath = imagePath;
            await Context.SaveChangesAsync();

            return imagePath;
        }
    }
}
