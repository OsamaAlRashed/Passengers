using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.DiscountDtos;
using Passengers.Main.DiscountService;
using Passengers.SharedKernel.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Swagger.ApiGroup;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [AppAuthorize(AppRoles.Shop)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository repository;

        public DiscountController(IDiscountRepository repository)
        {
            this.repository = repository;
        }

        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpGet]
        public async Task<IActionResult> Get() => await repository.Get().ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpGet]
        public async Task<IActionResult> GetActiveByProductId([Required] Guid productId) => await repository.GetActiveByProductId(productId).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpGet]
        public async Task<IActionResult> GetAllByProductId([Required] Guid productId) => await repository.GetAllByProductId(productId).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpPost]
        public async Task<IActionResult> Add(DiscountDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpPut]
        public async Task<IActionResult> Extension([Required] Guid productId, DateTime endDate) => await repository.EditEndDate(productId, endDate).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpPut]
        public async Task<IActionResult> End([Required] Guid productId) => await repository.EditEndDate(productId, null).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Delete(id).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpDelete]
        public async Task<IActionResult> DeleteActiveByProductId([Required] Guid productId) => await repository.DeleteActiveByProductId(productId).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpDelete]
        public async Task<IActionResult> DeleteAllByProductId([Required] Guid productId) => await repository.DeleteAllByProductId(productId).ToJsonResultAsync();
    }
}
