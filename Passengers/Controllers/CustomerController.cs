using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.CustomerDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.Security.CustomerService;
using Passengers.SharedKernel.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
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
        [HttpPatch]
        public async Task<IActionResult> Favorite([Required] Guid productId) => await repository.Favorite(productId).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpPatch]
        public async Task<IActionResult> Follow([Required] Guid productId) => await repository.Follow(productId).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpPatch]
        public async Task<IActionResult> UnFollow([Required] Guid productId) => await repository.UnFollow(productId).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpPatch]
        public async Task<IActionResult> UnFavorite([Required] Guid productId) => await repository.UnFavorite(productId).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpGet]
        public async Task<IActionResult> GetMyFavorite() => await repository.GetMyFavorite().ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [HttpGet]
        public async Task<IActionResult> GetShops([FromQuery] CustomerShopFilterDto filterDto, bool? topShops, int pageNumber, int pageSize) => await repository.GetShops(filterDto, topShops, pageNumber, pageSize).ToJsonResultAsync();

    }
}
