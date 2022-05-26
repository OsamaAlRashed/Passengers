using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.LocationDtos;
using Passengers.Location.AddressSerive;
using Passengers.SharedKernel.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Swagger.ApiGroup;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository repository;

        public AddressController(IAddressRepository countryRepository)
        {
            repository = countryRepository;
        }

        [ApiGroup(ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetByCustomerId([Required]Guid customerId) => await repository.GetByCustomerId(customerId).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Customer)]
        [AppAuthorize(AppRoles.Customer)]
        [HttpGet]
        public async Task<IActionResult> GetCustomerAddresses() => await repository.GetByCustomerId(null).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpGet]
        public async Task<IActionResult> GetByShopId([Required] Guid shopId) => await repository.GetByShopId(shopId).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Customer, ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Customer, AppRoles.Shop)]
        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Customer)]
        [AppAuthorize(AppRoles.Customer)]
        [HttpPost]
        public async Task<IActionResult> AddCustomerAddress(CustomerAddressDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Customer)]
        [AppAuthorize(AppRoles.Customer)]   
        [HttpPut]
        public async Task<IActionResult> UpdateCustomerAddress(CustomerAddressDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpPost]
        public async Task<IActionResult> AddShopAddress(ShopAddressDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpPut]
        public async Task<IActionResult> UpdateShopAddress(ShopAddressDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Customer, ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Customer, AppRoles.Shop)]
        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Remove(id).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Customer, ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop, AppRoles.Customer)]
        [HttpDelete]
        public async Task<IActionResult> DeleteByUserId([Required] Guid userId) => await repository.RemoveByUserId(userId).ToJsonResultAsync();

    }
}
