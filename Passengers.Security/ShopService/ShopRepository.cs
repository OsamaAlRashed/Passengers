using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.LocationDtos;
using Passengers.DataTransferObject.ProductDtos;
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
using Passengers.Shared.CategoryService;
using Passengers.Shared.DocumentService;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Services.CurrentUserService;
using Passengers.SharedKernel.Services.LangService.Contant;
using Passengers.SharedKernel.Services.LangService.LangErrorStore;
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
        private readonly IAccountRepository accountRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly IAddressRepository addressRepository;
        private readonly ICategoryRepository categoryRepository;

        public ShopRepository(PassengersDbContext context, UserManager<AppUser> userManager, IAccountRepository accountRepository, IDocumentRepository documentRepository, IAddressRepository addressRepository, ICategoryRepository categoryRepository) : base(context)
        {
            this.userManager = userManager;
            this.accountRepository = accountRepository;
            this.documentRepository = documentRepository;
            this.addressRepository = addressRepository;
            this.categoryRepository = categoryRepository;
        }

        public async Task<OperationResult<CreateShopAccountDto>> SignUp(CreateShopAccountDto dto)
        {
            if (await accountRepository.IsPhoneNumberUsed(dto.PhoneNumber))
                return _Operation.SetFailed<CreateShopAccountDto>("PhoneNumberUsed");

            AppUser shop = new()
            {
                PhoneNumber = dto.PhoneNumber,
                UserName = "Shop" + dto.PhoneNumber,
                Email = "Shop" + dto.PhoneNumber + "@Shop",
                Name = dto.Name,
                Description = dto.Decription,
                OwnerName = dto.OwnerName,
                UserType = UserTypes.Shop,
                AccountStatus = AccountStatus.WaitingCompleteInformation, ///ToDo Draft
                Address = new Address
                {
                    Lat = dto.Lat,
                    Long = dto.Long,
                    Text = dto.Address,
                    ///Todo Area Google Map
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
                var user = result.Result.User;
                var response = new
                {
                    user.Id,
                    user.UserName,
                    user.PhoneNumber,
                    result.Result.AccessToken,
                    result.Result.RefreshToken,
                    user.AccountStatus,
                    user.Name,
                    CategoryName = user.MainCategories.Select(x => x.Category?.Name).FirstOrDefault(),
                    ImagePath = user.Documents.Select(x => x.Path).FirstOrDefault()
                };
                return _Operation.SetSuccess<object>(response);
            }
            return _Operation.SetFailed<object>(result.Message,result.OperationResultType);
        }

        public async Task<OperationResult<object>> CompleteInfo(CompleteInfoShopDto dto)
        {
            var shop = await Context.Shops(AccountStatus.WaitingCompleteInformation).Where(x => x.Id == Context.CurrentUserId)
                .SingleOrDefaultAsync();
            if (shop == null)
                return _Operation.SetContent<object>(OperationResultTypes.NotExist, "ShopNotFound");

            var isValidFromTime = TimeSpan.TryParse(dto.FromTime, out var fromTime);
            var isValidToTime = TimeSpan.TryParse(dto.ToTime, out var toTime);

            if (!isValidFromTime || !isValidToTime)
                return _Operation.SetFailed<object>("TimeFormatIsNotValid");

            if(dto.Days != null && dto.Days.Count > 0)
            {
                var shopSchacule = new ShopSchedule
                {
                    Days = dto.Days.OrderBy(x => x).Select(i => i.ToString()).Aggregate((i, j) => i + "," + j),
                    FromTime = fromTime,
                    ToTime = toTime,
                    ShopId = shop.Id
                };

                Context.ShopSchedules.Add(shopSchacule);
            }

            var document = await documentRepository.Add(dto.Image, shop.Id, DocumentEntityTypes.Shop);
            
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
            Context.ShopCategories.Add(categoryShop);

            if(dto.TagIds != null)
            {
                List<Tag> newTags = new();
                foreach (var tagId in dto.TagIds)
                {
                    var currentTag = Context.Tags.Where(x => x.Id == tagId).SingleOrDefault();
                    if(currentTag != null && currentTag.ShopId != shop.Id)
                    {
                        var tag = new Tag
                        {
                            ShopId = shop.Id,
                            Name = currentTag.Name,
                            LogoPath = currentTag.LogoPath,
                        };
                        newTags.Add(tag);
                    }
                }
                Context.Tags.AddRange(newTags);
            }
            shop.AccountStatus = AccountStatus.Accepted;
            await Context.SaveChangesAsync();

            return _Operation.SetSuccess<object>(new
            {
                shop.Name,
                CategoryName = await categoryRepository.GetByShopId(shop.Id),
                ImagePath = document?.Path ?? string.Empty
            });
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
                .Include(x => x.MainCategories).ThenInclude(x => x.Category)
                .Include(x => x.ShopFavorites).Include(x => x.Tags).Include(x => x.Rates).Include(x => x.ShopContacts)
                .Include(x => x.Documents)
                .Where(x => x.Id == Context.CurrentUserId)
                .SingleOrDefaultAsync();
            if (shop == null)
                return _Operation.SetContent<ShopProfileDto>(OperationResultTypes.NotExist, "ShopNotFound");

            return _Operation.SetSuccess(new ShopProfileDto()
            {
                Name = shop.Name,
                CategoryName = shop.MainCategories.Select(x => x.Category?.Name).FirstOrDefault(),
                FollowerCount = shop.ShopFavorites.Count,
                ProductCount = shop.Tags.Sum(x => x.Products.Count),
                Rate = shop.Rates.ToList().CustomAverage(x => x.Degree),
                ImagePath = shop.Documents.Select(x => x.Path).FirstOrDefault(),
                Contacts = shop.ShopContacts.Select(x => new ContactInformationDto
                {
                    Text = x.Text,
                    Type = x.Type
                }).ToList(),
            });
        }

        public async Task<OperationResult<string>> UpdateImage(IFormFile image)
        {
            var shop = await Context.Shops().Where(x => x.Id == Context.CurrentUserId)
                .SingleOrDefaultAsync();
            if (shop == null)
                return _Operation.SetContent<string>(OperationResultTypes.NotExist, "");
            
            var document = await documentRepository.Update(image, shop.Id, DocumentEntityTypes.Shop);
            if (document == null)
                return _Operation.SetFailed<string>("UploadImageFailed");

            return document.Path;
        }

        public async Task<OperationResult<ShopDetailsDto>> Details()
        {
            var shop = await Context.Shops()
                .Include(x => x.MainCategories).ThenInclude(x => x.Category).Include(x => x.Address).Include(x => x.ShopContacts)
                .Where(x => x.Id == Context.CurrentUserId)
                .SingleOrDefaultAsync();
            if (shop == null)
                return _Operation.SetContent<ShopDetailsDto>(OperationResultTypes.NotExist, "ShopNotFound");

            return _Operation.SetSuccess(new ShopDetailsDto
            {
                Name = shop.Name,
                Address = shop.Address?.Text,
                Lat = shop.Address?.Lat,
                Long = shop.Address?.Long,
                CategoryId = shop.MainCategories.Select(x => x.CategoryId).FirstOrDefault(),
                CategoryName = shop.MainCategories.Select(x => x.Category.Name).FirstOrDefault(),
                Contacts = shop.ShopContacts.Select(x => new ContactInformationDto
                {
                    Text = x.Text,
                    Type = x.Type
                }).ToList(),
            });
        }

        public async Task<OperationResult<ShopDetailsDto>> Update(ShopDetailsDto dto)
        {
            var shop = await Context.Shops()
                .Include(x => x.MainCategories).Include(x => x.Address).Include(x => x.ShopContacts)
                .Where(x => x.Id == Context.CurrentUserId)
                .SingleOrDefaultAsync();
            if (shop == null)
                return _Operation.SetContent<ShopDetailsDto>(OperationResultTypes.NotExist, "ShopNotFound");
            
            shop.Name = dto.Name;

            var addressDto = new AddressDto
            {
                EntityId = shop.Id,
                Lat = dto.Lat,
                Long = dto.Long,
                Text = dto.Address,
                Type = AddressTypes.Shop,
            };

            if (shop.Address == null)
            {
                await addressRepository.Add(addressDto);
            }
            else
            {
                addressDto.Id = shop.Address.Id;
                addressDto.AreaId = shop.Address.AreaId;

                await addressRepository.UpdateShopAddress(addressDto);
            }

            if(dto.Contacts != null)
            {
                await UpdateContracts(shop, dto.Contacts);
            }

            if (!Context.ShopCategories.Where(x => x.CategoryId == dto.CategoryId && x.ShopId == shop.Id).Any())
            {
                Context.ShopCategories.RemoveRange(shop.MainCategories);

                var categoryShop = new ShopCategory
                {
                    ShopId = shop.Id,
                    CategoryId = dto.CategoryId
                };
                Context.ShopCategories.Add(categoryShop);
            }

            await Context.SaveChangesAsync();
            return dto;
        }

        private async Task UpdateContracts(AppUser shop, List<ContactInformationDto> contacts)
        {
            var shopContacts = contacts.Select(x => new ShopContact
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
                if (contactEntity != null)
                    contactEntity.Text = contact.Text;
            }

            Context.ShopContacts.RemoveRange(removedContacts);
            Context.ShopContacts.AddRange(addedContacts);
        }

        public async Task<OperationResult<ShopHomeDto>> Home()
        {
            var topProducts = await Context.Products.Where(x => x.Tag.ShopId == Context.CurrentUserId)
                .Include(x => x.Rates).Include(x => x.Documents).Include(x => x.OrderDetails)
                .OrderByDescending(x => x.Rates.Any() ? x.Rates.Average(x => x.Degree) : 0)
                .Take(5).ToListAsync();

            var popularProducts = await Context.Products.Where(x => x.Tag.ShopId == Context.CurrentUserId)
                .Include(x => x.Rates).Include(x => x.Documents).Include(x => x.OrderDetails)
                .OrderByDescending(x => x.OrderDetails.Sum(x => x.Quantity))
                .Take(5).ToListAsync();

            var topOffers = await Context.Offers.Where(x => x.ShopId == Context.CurrentUserId && x.EndDate > DateTime.Now)
                .Include(x => x.Documents).Include(x => x.OrderDetails)
                .OrderByDescending(x => x.OrderDetails.Sum(x => x.Quantity))
                .Take(5).ToListAsync();

            return _Operation.SetSuccess(new ShopHomeDto
            {
                TopOffers = topOffers.Select(x => new ItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImagePath = x.ImagePath,
                    Buyers = x.Buyers,
                }).ToList(),
                TopProducts = topProducts.Select(x => new ItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImagePath = x.ImagePath,
                    Buyers = x.Buyers,
                    Rate = x.Rate,
                }).ToList(),
                PopularProducts = popularProducts.Select(x => new ItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImagePath = x.ImagePath,
                    Buyers = x.Buyers,
                    Rate = x.Rate,
                }).ToList(),
            });
        }

        public async Task<OperationResult<bool>> UpdateWorkingDays(List<int> days)
        {
            if(days == null || days.Count == 0)
                return _Operation.SetFailed<bool>(LangErrorStore.Get(ErrorCodeConstant.ChooseDayAtLeast, Context.CurrentLang));

            var schedule = await Context.ShopSchedules.Where(x => x.ShopId == Context.CurrentUserId.Value).FirstOrDefaultAsync();


            if (schedule == null)
            {
                schedule = new ShopSchedule()
                {
                    ShopId = Context.CurrentUserId.Value
                };
                Context.ShopSchedules.Add(schedule);
            }

            schedule.Days = days.OrderBy(x => x).Select(i => i.ToString()).Aggregate((i, j) => i + "," + j);

            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> UpdateWorkingTimes(string fromTime, string toTime)
        {
            var isValidFromTime = TimeSpan.TryParse(fromTime, out var fromTimeSpan);
            var isValidToTime = TimeSpan.TryParse(toTime, out var toTimeSpan);

            if (!isValidFromTime || !isValidToTime)
                return _Operation.SetFailed<bool>(LangErrorStore.Get(ErrorCodeConstant.TimeFormatIsNotValid, Context.CurrentLang));

            var schedule = await Context.ShopSchedules.Where(x => x.ShopId == Context.CurrentUserId.Value).FirstOrDefaultAsync();

            if (schedule == null)
            {
                schedule = new ShopSchedule()
                {
                    ShopId = Context.CurrentUserId.Value
                };
                Context.ShopSchedules.Add(schedule);
            }


            schedule.FromTime = fromTimeSpan;
            schedule.ToTime = toTimeSpan;

            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<WorkingDaysDto>> GetWorkingDays()
        {
            var schedule = await Context.ShopSchedules.Where(x => x.ShopId == Context.CurrentUserId.Value).FirstOrDefaultAsync();
            if (schedule == null)
                return _Operation.SetContent<WorkingDaysDto>(OperationResultTypes.NotExist, "");

            return _Operation.SetSuccess(new WorkingDaysDto
            {
                Days = schedule.Days.Split(",").Select(x => int.Parse(x)).ToList(),
                FromTime = schedule.FromTime.ToString(),
                ToTime = schedule.ToTime.ToString()
            });

        }
    }
}
