using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Passengers.DataTransferObject.ProductDtos;
using Passengers.Main.ProductService;
using Passengers.Security.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Pagnation;
using Passengers.SharedKernel.Swagger.ApiGroup;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [ApiGroup(ApiGroupNames.Shop)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository repository;

        public ProductController(IProductRepository repository)
        {
            this.repository = repository;
        }

        [AppAuthorize(AppRoles.Shop)]
        [HttpPost]
        public async Task<IActionResult> Add([FromForm]SetProductDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Shop)]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm]SetProductDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Shop)]
        [HttpPut]
        public async Task<IActionResult> ChangePrice([Required]Guid id, decimal newPrice) => await repository.ChangePrice(id, newPrice).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Shop)]
        [HttpPut]
        public async Task<IActionResult> ChangeAvilable([Required] Guid id) => await repository.ChangeAvilable(id).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Shop)]
        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Shop)]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductFilterDto filterDto, SortProductTypes? sortType, bool? isDes, int pageNumber = 1, int pageSize = 10)
            => await repository.Get(filterDto, sortType, isDes, pageNumber, pageSize).AddPagnationHeaderAsync(this).ToJsonResultAsync();
        
        [AppAuthorize(AppRoles.Shop)]
        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Remove(id).ToJsonResultAsync();

    }
}
