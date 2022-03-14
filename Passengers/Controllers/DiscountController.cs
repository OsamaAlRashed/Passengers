using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.DiscountDtos;
using Passengers.Main.DiscountService;
using Passengers.SharedKernel.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
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

        [HttpGet]
        public async Task<IActionResult> Get() => await repository.Get().ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetActiveByProductId([Required] Guid productId) => await repository.GetActiveByProductId(productId).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetAllByProductId([Required] Guid productId) => await repository.GetAllByProductId(productId).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Add(DiscountDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Extension([Required] Guid productId, DateTime endDate) => await repository.EditEndDate(productId, endDate).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> End([Required] Guid productId) => await repository.EditEndDate(productId, null).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Delete(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> DeleteActiveByProductId([Required] Guid productId) => await repository.DeleteActiveByProductId(productId).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> DeleteAllByProductId([Required] Guid productId) => await repository.DeleteAllByProductId(productId).ToJsonResultAsync();
    }
}
