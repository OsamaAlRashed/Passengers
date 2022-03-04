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
        public async Task<IActionResult> GetByCustomerId([Required]Guid customerId) => await repository.GetByCustomerId(customerId).ToJsonResultAsync();
        
        [HttpGet]
        public async Task<IActionResult> GetByShopId([Required] Guid shopId) => await repository.GetByShopId(shopId).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Add(AddressDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Update(AddressDto dto) => await repository.UpdateShopAddress(dto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Remove(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> DeleteByUserId([Required] Guid userId) => await repository.RemoveByUserId(userId).ToJsonResultAsync();

    }
}
