using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.CustomerDtos;
using Passengers.Security.CustomerService;
using Passengers.Security.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Pagnation;
using Passengers.SharedKernel.Swagger.ApiGroup;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [ApiGroup(ApiGroupNames.Customer)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository repository;

        public CustomerController(ICustomerRepository repository)
        {
            this.repository = repository;
        }

        //[ProducesResponseType(typeof(List<GetAllCustomerDto>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginCustomerDto dto) => await repository.Login(dto).ToJsonResultAsync();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignUp(CreateAccountCustomerDto dto) => await repository.SignUp(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpPut]
        public async Task<IActionResult> UpdateInformation(CustomerInformationDto dto) => await repository.UpdateInformation(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpPatch]
        public async Task<IActionResult> UploadImage(IFormFile file) => await repository.UploadImage(file).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpGet]
        public async Task<IActionResult> Details() => await repository.Details().ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpGet]
        public async Task<IActionResult> GetProfile() => await repository.GetProfile().ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpPatch]
        public async Task<IActionResult> Favorite([Required] Guid productId) => await repository.Favorite(productId).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpPatch]
        public async Task<IActionResult> UnFavorite([Required] Guid productId) => await repository.UnFavorite(productId).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpPatch]
        public async Task<IActionResult> Follow([Required] Guid shopId) => await repository.Follow(shopId).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpPatch]
        public async Task<IActionResult> UnFollow([Required] Guid shopId) => await repository.UnFollow(shopId).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpGet]
        public async Task<IActionResult> GetMyFavorite(string search, int pageNumber = 1, int pageSize = 10) 
            => await repository.GetMyFavorite(search, pageNumber, pageSize).AddPagnationHeaderAsync(this).ToJsonResultAsync();

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetShops([FromQuery] CustomerShopFilterDto filterDto, bool? topShops, int pageNumber = 1, int pageSize = 10)
            => await repository.GetShops(filterDto, topShops, pageNumber, pageSize).AddPagnationHeaderAsync(this).ToJsonResultAsync();

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] CustomerProductFilterDto filterDto, int pageNumber = 1, int pageSize = 10)
            => await repository.GetProducts(filterDto, pageNumber, pageSize).AddPagnationHeaderAsync(this).ToJsonResultAsync();

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Home() => await repository.Home().ToJsonResultAsync();

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ProductDetails([Required] Guid productId) => await repository.ProductDetails(productId).ToJsonResultAsync();

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ShopDetails([Required] Guid shopId) => await repository.ShopDetails(shopId).ToJsonResultAsync();

    }
}
