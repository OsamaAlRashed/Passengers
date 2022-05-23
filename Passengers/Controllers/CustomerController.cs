using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.CustomerDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.Security.CustomerService;
using Passengers.SharedKernel.Attribute;
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
    [AppAuthorize(AppRoles.Customer)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository repository;

        public CustomerController(ICustomerRepository repository)
        {
            this.repository = repository;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginCustomerDto dto) => await repository.Login(dto).ToJsonResultAsync();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignUp(CreateAccountCustomerDto dto) => await repository.SignUp(dto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> UpdateInformation(CustomerInformationDto dto) => await repository.UpdateInformation(dto).ToJsonResultAsync();

        [HttpPatch]
        public async Task<IActionResult> UploadImage(IFormFile file) => await repository.UploadImage(file).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> Details() => await repository.Details().ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetProfile() => await repository.GetProfile().ToJsonResultAsync();

        [HttpPatch]
        public async Task<IActionResult> Favorite([Required] Guid productId) => await repository.Favorite(productId).ToJsonResultAsync();

        [HttpPatch]
        public async Task<IActionResult> Follow([Required] Guid shopId) => await repository.Follow(shopId).ToJsonResultAsync();

        [HttpPatch]
        public async Task<IActionResult> UnFollow([Required] Guid shopId) => await repository.UnFollow(shopId).ToJsonResultAsync();

        [HttpPatch]
        public async Task<IActionResult> UnFavorite([Required] Guid productId) => await repository.UnFavorite(productId).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetMyFavorite(string search, int pageNumber = 1, int pageSize = 10) 
            => await repository.GetMyFavorite(search, pageNumber, pageSize).AddPagnationHeaderAsync(this).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetShops([FromQuery] CustomerShopFilterDto filterDto, bool? topShops, int pageNumber = 1, int pageSize = 10)
            => await repository.GetShops(filterDto, topShops, pageNumber, pageSize).AddPagnationHeaderAsync(this).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] CustomerProductFilterDto filterDto, int pageNumber = 1, int pageSize = 10)
            => await repository.GetProducts(filterDto, pageNumber, pageSize).AddPagnationHeaderAsync(this).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> Home() => await repository.Home().ToJsonResultAsync();

    }
}
