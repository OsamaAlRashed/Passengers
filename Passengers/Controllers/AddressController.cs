using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.LocationDtos;
using Passengers.Location.AddressSerive;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetByCustomerId([Required]Guid customerId) => await repository.GetByCustomerId(customerId).ToJsonResultAsync();
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetByShopId([Required] Guid shopId) => await repository.GetByShopId(shopId).ToJsonResultAsync();

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetById([Required] Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(AddressDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update(AddressDto dto) => await repository.UpdateShopAddress(dto).ToJsonResultAsync();

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Remove(id).ToJsonResultAsync();

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteByUserId([Required] Guid userId) => await repository.RemoveByUserId(userId).ToJsonResultAsync();

    }
}
