using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.LocationDtos;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.DataTransferObject.ShopDtos;
using Passengers.Location.AddressSerive;
using Passengers.Models.Location;
using Passengers.Models.Main;
using Passengers.Models.Security;
using Passengers.Models.Shared;
using Passengers.Repository.Base;
using Passengers.Security.AccountService;
using Passengers.Security.Shared.Store;
using Passengers.Shared.DocumentService;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Services.CurrentUserService;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.ShopService
{
    public class ShopRepository : BaseRepository, IShopRepository
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly IAccountRepository accountRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IAddressRepository addressRepository;

        public ShopRepository(PassengersDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IAccountRepository accountRepository, IDocumentRepository documentRepository, ICurrentUserService currentUserService, IAddressRepository addressRepository) : base(context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.accountRepository = accountRepository;
            this.documentRepository = documentRepository;
            this.currentUserService = currentUserService;
            this.addressRepository = addressRepository;
        }

        public async Task<OperationResult<CreateShopAccountDto>> SignUp(CreateShopAccountDto dto)
        {
            AppUser shop = new()
            {
                PhoneNumber = dto.PhoneNumber,
                UserName = "Shop" + dto.PhoneNumber,
                Email = "Shop" + dto.PhoneNumber + "@Shop",
                Name = dto.Name,
                Description = dto.Decription,
                OwnerName = dto.OwnerName,
                UserType = UserTypes.Shop,
                AccountStatus = AccountStatus.Draft,
                Address = new Address
                {
                    Lat = dto.Lat,
                    Long = dto.Long,
                    Text = dto.Address,
                    AreaId = Context.Areas.FirstOrDefault().Id
                }
            };
            var identityResult = await userManager.CreateAsync(shop, dto.Password);

            if (!identityResult.Succeeded)
                return _Operation.SetFailed<CreateShopAccountDto>(String.Join(",", identityResult.Errors.Select(error => error.Description)));

            var roleIdentityResult = await userManager.AddToRoleAsync(shop, UserTypes.Shop.ToString());

            if (!roleIdentityResult.Succeeded)
                return _Operation.SetFailed<CreateShopAccountDto>(String.Join(",", roleIdentityResult.Errors.Select(error => error.Description)));

            await Context.SaveChangesAsync();
            dto.Id = shop.Id;
            return _Operation.SetSuccess(dto);
        }

        public async Task<OperationResult<object>> Login(LoginMobileDto dto)
        {
            var result = await accountRepository.Login(SecurityStore.Query.ShopToBaseLoginDto(dto));
            if (result.IsSuccess)
            {
                var response = new
                {
                    Id = result.Result.Id,
                    result.Result.UserName,
                    result.Result.PhoneNumber,
                    result.Result.AccessToken,
                    result.Result.RefreshToken,
                };
                return _Operation.SetSuccess<object>(response);
            }
            return _Operation.SetFailed<object>(result.Message,result.OperationResultType);
        }

        public async Task<OperationResult<bool>> CompleteInfo(CompleteInfoShopDto dto)
        {
            var shop = await Context.Shops(AccountStatus.WaitingCompleteInformation).Where(x => x.Id == currentUserService.UserId)
                .SingleOrDefaultAsync();
            if (shop == null)
                return (OperationResultTypes.NotExist, "ShopNotFound");

            await documentRepository.Add(dto.Image, dto.Id, DocumentEntityTypes.Shop);

            if(dto.Days != null && dto.Days.Count > 0)
            {
                var shopSchacule = new ShopSchedule
                {
                    Days = dto.Days.Select(i => i.ToString()).Aggregate((i, j) => i + "," + j),
                    FromTime = dto.FromTime,
                    ToTime = dto.ToTime,
                    ShopId = dto.Id,
                };
            }

            if(dto.Contacts != null)
            {
                foreach (var contact in dto.Contacts)
                {
                    var entityContact = new ShopContact
                    {
                        Text = contact.Text,
                        Type = contact.Type,
                        ShopId = shop.Id,
                    };
                }
            }

            var categoryShop = new ShopCategory
            {
                ShopId = shop.Id,
                CategoryId = dto.CategoryId
            };

            if(dto.TagIds != null)
            {
                foreach (var tagId in dto.TagIds)
                {
                    var publicTag = Context.Tags.Where(x => x.Id == tagId).SingleOrDefault();
                    var tag = new Tag
                    {
                        ShopId = shop.Id,
                        Name = publicTag?.Name ?? "NoLabel",
                        LogoPath = publicTag?.LogoPath,
                    };
                }
            }
            shop.AccountStatus = AccountStatus.Accepted;
            await Context.SaveChangesAsync();

            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<List<ShopDto>>> Get(AccountStatus accountStatus)
        {
            var result = await Context.Shops(accountStatus).Select(x => new ShopDto
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name
            }).ToListAsync();
            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<ShopProfileDto>> GetProfile()
        {
            var shop = await Context.Shops()
                .Include(x => x.MainCategories).Include(x => x.ShopFavorites).Include(x => x.Tags).Include(x => x.Rates).Include(x => x.ShopContacts)
                .Where(x => x.Id == currentUserService.UserId.Value)
                .SingleOrDefaultAsync();
            if (shop == null)
                return (OperationResultTypes.NotExist, "");

            return new ShopProfileDto()
            {
                Name = shop.Name,
                CategoryName = shop.MainCategories.Select(x => x.Category.Name).FirstOrDefault(),
                FollowerCount = shop.ShopFavorites.Count,
                ProductCount = shop.Tags.Sum(x => x.Products.Count),
                Rate = shop.Rates.Average(x => x.Degree),
                Contacts = shop.ShopContacts.Select(x => new ContactInformationDto
                {
                    Text = x.Text,
                    Type = x.Type
                }).ToList(),
            };
        }

        public async Task<OperationResult<string>> UpdateImage(IFormFile image)
        {
            var shop = await Context.Shops().Where(x => x.Id == currentUserService.UserId.Value)
                .SingleOrDefaultAsync();
            if (shop == null)
                return (OperationResultTypes.NotExist, "");
            
            var document = await documentRepository.Update(image, shop.Id, DocumentEntityTypes.Shop);
            if(document == null)
                return (OperationResultTypes.Failed, "");

            return document.Path;
        }

        public async Task<OperationResult<ShopDetailsDto>> Details()
        {
            var shop = await Context.Shops()
                .Include(x => x.MainCategories).Include(x => x.Address).Include(x => x.ShopContacts)
                .Where(x => x.Id == currentUserService.UserId.Value)
                .SingleOrDefaultAsync();
            if (shop == null)
                return (OperationResultTypes.NotExist, "");

            return new ShopDetailsDto
            {
                Name = shop.Name,
                Address = shop.Address.Text,
                Lat = shop.Address.Lat,
                Long = shop.Address.Long,
                CategoryId = shop.MainCategories.Select(x => x.CategoryId).FirstOrDefault(),
                CategoryName = shop.MainCategories.Select(x => x.Category.Name).FirstOrDefault(),
                Contacts = shop.ShopContacts.Select(x => new ContactInformationDto
                {
                    Text = x.Text,
                    Type = x.Type
                }).ToList(),
            };
        }

        public async Task<OperationResult<ShopDetailsDto>> Update(ShopDetailsDto dto)
        {
            var shop = await Context.Shops()
                .Include(x => x.MainCategories).Include(x => x.Address).Include(x => x.ShopContacts)
                .Where(x => x.Id == currentUserService.UserId.Value)
                .SingleOrDefaultAsync();
            if (shop == null)
                return (OperationResultTypes.NotExist, "");
            
            shop.Name = dto.Name;

            await addressRepository.UpdateShopAddress(new AddressDto
            {
                EntityId = shop.Id,
                Lat = dto.Lat,
                Long = dto.Long,
                Text = dto.Address,
                Type = AddressTypes.Shop,
                Id = shop.Address.Id,
                AreaId = shop.Address.AreaId
            });

            var shopContacts = dto.Contacts.Select(x => new ShopContact
            {
                Text = x.Text,
                Type = x.Type,
                ShopId = shop.Id
            });

            var (addedContacts, removedContacts) = shop.ShopContacts.IsolatedExceptOldNew(shopContacts, old => old.Type, @new => @new.Type);

            var updatedContacts = shopContacts.Except(addedContacts.Union(removedContacts));

            foreach (var contact in updatedContacts)
            {
                var contactEntity = shop.ShopContacts.Where(x => x.Type == contact.Type).FirstOrDefault();
                if(contactEntity != null)
                    contactEntity.Text = contact.Text;
            }

            Context.ShopContacts.RemoveRange(removedContacts);
            Context.ShopContacts.AddRange(addedContacts);

            await Context.SaveChangesAsync();
            return dto;
        }
    }
}
