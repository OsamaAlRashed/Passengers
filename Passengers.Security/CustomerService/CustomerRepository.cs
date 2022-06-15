using Microsoft.AspNetCore.Hosting;
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
            var isExist = await Context.Favorites
                .Where(x => x.ProductId == productId && x.CustomerId == Context.CurrentUserId.Value)
                .AnyAsync();

            if (!isExist)
            {
                var fav = new Favorite
                {
                    ProductId = productId,
                    CustomerId = Context.CurrentUserId.Value
                };

                Context.Favorites.Add(fav);
                await Context.SaveChangesAsync();
            }
            
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> Follow(Guid shopId)
        {
            var isExist = await Context.Favorites
                .Where(x => x.ShopId == shopId && x.CustomerId == Context.CurrentUserId.Value)
                .AnyAsync();

            if (!isExist)
            {
                var fav = new Favorite
                {
                    ShopId = shopId,
                    CustomerId = Context.CurrentUserId.Value
                };

                Context.Favorites.Add(fav);
                await Context.SaveChangesAsync();
            }
                
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<PagedList<GetProductDto>>> GetMyFavorite(string search, int pageNumber = 1, int pageSize = 10)
        {
            var products = await Context.Products
                .Include(x => x.Documents).Include(x => x.OrderDetails).Include(x => x.Reviews).Include(x => x.Discounts)
                .Where(x => (string.IsNullOrEmpty(search) || x.Name.Contains(search)) 
                    && x.Favorites.Select(x => x.CustomerId).Contains(Context.CurrentUserId.Value))
                .Select(CustomerStore.Query.GetSelectProduct(Context.CurrentUserId))
                .ToPagedListAsync(pageNumber, pageSize);

            return _Operation.SetSuccess(products);
        }

        public async Task<OperationResult<object>> GetProductById(Guid id)
        {
            var product = await Context.Products
                .Include(x => x.Tag)
                .Include(x => x.Discounts)
                .Include(x => x.Reviews)
                .ThenInclude(x => x.Customer)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (product == null)
                return _Operation.SetContent<object>(OperationResultTypes.NotExist, "ProductNotFound");
             
            //var discount = GetCurrentDiscount(product.Discounts);

            var result = new
            {
                product.Id,
                ImagePath = product.Documents.Select(x => x.Path).FirstOrDefault(),
                product.TagId,
                TagName = product.Tag.Name,
                product.Name,
                product.Price,
                //DiscountPrice = discount?.Price,
                product.Avilable,
                product.PrepareTime,
                Rate = product.Rate,
                RateNumber = product.Reviews.Count,
                product.Description,
                product.Tag.Shop.OrderStatus,
                ShopName = product.Tag.Shop.Name,
                ShopId = product.Tag.Shop.Id,
                ShopCategory = product.Tag.Shop.Category.Name,
                ShopImagePath = product.Tag.Shop.Documents.Select(x => x.Path).FirstOrDefault(),
                ShopOnline = product.Tag.Shop.ShopSchedules.Any(x => x.Days.Contains(DateTime.Now.Day.ToString()) && x.FromTime <= DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay <= x.ToTime),
                Reviews = product.Reviews.Select(r => new
                {
                    r.Id,
                    r.Rate,
                    r.Descreption,
                    CustomerName = r.Customer.Name,
                    r.CustomerId,
                    Date = r.DateCreated
                }).OrderByDescending(x => x.Date).Take(3).ToList(),
            };
            return _Operation.SetSuccess<object>(result);
        }

        public async Task<OperationResult<PagedList<GetProductDto>>> GetProducts(CustomerProductFilterDto filterDto, int pageNumber = 1, int pageSize = 10)
        {
            var products = await Context.Products
                .Include(x => x.Documents).Include(x => x.OrderDetails).Include(x => x.Reviews).Include(x => x.Discounts)
                .Where(CustomerStore.Filter.WhereFilterProduct(filterDto))
                .Select(CustomerStore.Query.GetSelectProduct(Context.CurrentUserId))
                .ToPagedListAsync(pageNumber, pageSize);

            return _Operation.SetSuccess(products);
        }

        public async Task<OperationResult<object>> GetProfile()
        {
            var customer = await Context.Customers().Where(x => x.Id == Context.CurrentUserId)
                .SingleOrDefaultAsync();
            if (customer == null)
                return _Operation.SetContent<CustomerInformationDto>(OperationResultTypes.NotExist, "");

            return _Operation.SetSuccess<object>(new
            {
                ImagePath = customer.IdentifierImagePath,
                Name = customer.FullName,
                customer.PhoneNumber
            });
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
                .Include(x => x.Category)
                .Include(x => x.Documents).Include(x => x.ShopSchedules)
                .Include(x => x.Tags).ThenInclude(x => x.Products).ThenInclude(x => x.Reviews)
                .Where(CustomerStore.Filter.WhereFilterShop(filterDto, shopIds))
                .SortBy(CustomerStore.Query.SortShop(topShops),true)
                .Select(CustomerStore.Query.ShopToShopCustomerDto)
                .ToPagedListAsync(pageNumber, pageSize);

            return _Operation.SetSuccess(shops);
        }

        public async Task<OperationResult<CustomerHomeDto>> Home()
        {
            var products = await Context.Products
                .Include(x => x.Reviews).Include(x => x.Documents).Include(x => x.OrderDetails).Include(x => x.OrderDetails)
                .Include(x => x.Tag).ThenInclude(x => x.Shop).ThenInclude(x => x.ShopSchedules)
                .Include(x => x.Tag).ThenInclude(x => x.Shop).ThenInclude(x => x.Documents)
                .ToListAsync();

            var homeDto = new CustomerHomeDto
            {
                NewShops = await Context.Shops()
                    .OrderByDescending(x => x.DateCreated)
                    .Include(x => x.Documents).Include(x => x.ShopSchedules)
                    .Take(5).Select(CustomerStore.Query.ShopToShopDto)
                    .ToListAsync(),
                TopProducts = products
                    .OrderByDescending(x => x.Reviews.Any() ? x.Reviews.Average(x => x.Rate) : 0)
                    .Take(5).Select(CustomerStore.Query.ProductToProductDto).ToList(),
                NewProducts = products
                    .OrderByDescending(x => x.DateCreated)
                    .Take(5).Select(CustomerStore.Query.ProductToProductDto).ToList(),
                PopularProducts = products
                    .OrderByDescending(x => x.OrderDetails.Sum(x => x.Quantity))
                    .Take(5).Select(CustomerStore.Query.ProductToProductDto).ToList(),
                SuggestionProducts = products
                    .OrderByDescending(x => Guid.NewGuid())
                    .Take(5).Select(CustomerStore.Query.ProductToProductDto).ToList()
            };

            return _Operation.SetSuccess<CustomerHomeDto>(homeDto);
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

        public async Task<OperationResult<object>> ProductDetails(Guid productId)
        {
            var product = await Context.Products
                .Where(x => x.Id == productId)
                .Include("Tag.Shop.ShopSchedules")
                .Include("Tag.Shop.ShopFavorites")
                .Include("Tag.Shop.Documents")
                .Include("Tag.Shop.Category")
                .Include(x => x.Discounts)
                .Include(x => x.Reviews)
                .ThenInclude(x => x.Customer)
                .Include(x => x.Documents)
                .SingleOrDefaultAsync();

            if (product == null)
                return _Operation.SetContent<object>(OperationResultTypes.NotExist, "ProductNotFound");

            var result = new
            {
                product.Id,
                ImagePath = product.Documents.Select(x => x.Path).FirstOrDefault(),
                product.TagId,
                TagName = product.Tag.Name,
                product.Name,
                product.Price,
                HasDiscount = product.Discounts.Where(x => x.StartDate <= DateTime.Now && DateTime.Now <= x.EndDate).FirstOrDefault() is null ? false : true,
                DiscountPrice = product.Discounts.Where(x => x.StartDate <= DateTime.Now && DateTime.Now <= x.EndDate).FirstOrDefault()?.Price,
                product.Avilable,
                product.PrepareTime,
                RateDegree = product.Rate,
                RateNumber = product.Reviews.Count,
                product.Description,
                DeliveryAvilable = product.Tag.Shop.OrderStatus ?? true,
                Shop = new
                {
                    Id = product.Tag.ShopId,
                    Name = product.Tag.Shop.Name,
                    Online = product.Tag.Shop.ShopSchedules.Any(x => x.Days.Contains(DateTime.Now.Day.ToString()) && x.FromTime <= DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay <= x.ToTime),
                    Follow = product.Tag.Shop.ShopFavorites.Any(x => x.CustomerId == Context.CurrentUserId),
                    ImagePath = product.Tag.Shop.Documents.Select(x => x.Path).FirstOrDefault(),
                    CategoryName = product.Tag.Shop.Category.Name
                },
                Reviews = product.Reviews.Select(r => new
                {
                    r.Id,
                    r.Rate,
                    r.Descreption,
                    CustomerName = r.Customer.Name,
                    r.CustomerId,
                    Date = r.DateCreated
                }).OrderByDescending(x => x.Date).Take(3).ToList(),
            };
            return _Operation.SetSuccess<object>(result);   
        }

        public async Task<OperationResult<object>> ShopDetails(Guid shopId)
        {
            var shop = await Context.Shops()
                .Where(x => x.Id == shopId)
                .Include(x => x.ShopSchedules)
                .Include(x => x.ShopFavorites)
                .Include(x => x.Documents)
                .Include(x => x.Category)
                .Include(x => x.ShopContacts)
                .Include(x => x.Address)
                .Include("Tags.Products.Reviews")
                .SingleOrDefaultAsync();

            if (shop == null)
                return _Operation.SetContent<object>(OperationResultTypes.NotExist, "ShopNotFound");

            var result = new
            {
                Id = shopId,
                Name = shop.Name,
                Online = shop.ShopSchedules.Any(x => x.Days.Contains(DateTime.Now.Day.ToString()) && x.FromTime <= DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay <= x.ToTime),
                Follow = shop.ShopFavorites.Any(x => x.CustomerId == Context.CurrentUserId),
                ImagePath = shop.Documents.Select(x => x.Path).FirstOrDefault(),
                CategoryName = shop.Category.Name,
                FromDay = shop.ShopSchedules.Any() ?
                        (shop.ShopSchedules.FirstOrDefault().Days.IsNullOrEmpty() ?
                            0 : (shop.ShopSchedules.FirstOrDefault().Days[0] - 49))
                                : 0,
                ToDay = shop.ShopSchedules.Any() ?
                        (shop.ShopSchedules.FirstOrDefault().Days.IsNullOrEmpty() ?
                            0 : (shop.ShopSchedules.FirstOrDefault().Days[shop.ShopSchedules.FirstOrDefault().Days.Length - 1] - 49))
                                : 0,
                FromTime = shop.ShopSchedules.FirstOrDefault().FromTime.ToString(@"hh\:mm"),
                ToTime = shop.ShopSchedules.FirstOrDefault().ToTime.ToString(@"hh\:mm"),
                Contacts = shop.ShopContacts.Select(xx => new
                {
                    xx.Type,
                    xx.Text,
                }).ToList(),
                Address = new
                {
                    Lat = shop.Address.Lat,
                    Long = shop.Address.Long,
                    Text = shop.Address.Text
                },
                OrderStatus = shop.OrderStatus ?? true,
                Rate = shop.Tags.SelectMany(x => x.Products.SelectMany(x => x.Reviews)).Any() ?
                    shop.Tags.SelectMany(x => x.Products.SelectMany(x => x.Reviews)).Average(x => x.Rate) : 0
            };

            return _Operation.SetSuccess<object>(result);
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
