using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.LocationDtos;
using Passengers.Location.CityService;
using Passengers.Security.Attribute;
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
    [ApiGroup(ApiGroupNames.Test)]
    [AppAuthorize(AppRoles.Admin)]
    public class CityController : ControllerBase
    {
        private readonly ICityRepository repository;

        public CityController(ICityRepository countryRepository)
        {
            repository = countryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => await repository.Get().ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Add(CityDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Update(CityDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required]Guid id) => await repository.Remove(id).ToJsonResultAsync();

    }
}
