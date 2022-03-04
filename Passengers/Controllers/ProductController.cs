using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.ProductDtos;
using Passengers.Main.ProductService;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository repository;

        public ProductController(IProductRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => await repository.Get().ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Add(SetProductDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Update(SetProductDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> ChangePrice([Required]Guid id, decimal newPrice) => await repository.ChangePrice(id, newPrice).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> ChangeAvilable([Required] Guid id) => await repository.ChangeAvilable(id).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetFoodMenu(Guid tagId, int pageSize, int pageNumber) => await repository.GetFoodMenu(tagId, pageSize, pageNumber).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Remove(id).ToJsonResultAsync();

    }
}
