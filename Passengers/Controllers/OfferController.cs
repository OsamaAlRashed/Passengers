using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.OfferDtos;
using Passengers.Main.OfferService;
using Passengers.SharedKernel.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.Enums;
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
    [AppAuthorize(AppRoles.Shop)]
    public class OfferController : ControllerBase
    {
        private readonly IOfferRepository repository;

        public OfferController(IOfferRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(OfferTypes type, int pageSize, int pageNumber) => await repository.Get(type, pageSize, pageNumber).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Add(SetOfferDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Update(SetOfferDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Extension([Required] Guid id, DateTime endDate) => await repository.Extension(id, endDate).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> End([Required] Guid id) => await repository.Extension(id, null).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Remove(id).ToJsonResultAsync();
    }
}
