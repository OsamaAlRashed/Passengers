using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.SharedDtos.CategoryDtos;
using Passengers.Shared.CategoryService;
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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository repository;

        public CategoryController(ICategoryRepository countryRepository)
        {
            repository = countryRepository;
        }

        [ApiGroup(ApiGroupNames.Shop, ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Shop, AppRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Get() => await repository.Get().ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpGet]
        public async Task<IActionResult> GetById([Required]Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] SetCategoryDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpPost]
        public async Task<IActionResult> Update([FromForm] SetCategoryDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Remove(id).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpGet]
        public async Task<IActionResult> GetRoots() => await repository.GetRoots().ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpGet]
        public async Task<IActionResult> GetChildern([Required] Guid id) => await repository.GetChildern(id).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpGet]
        public async Task<IActionResult> GetFullPath([Required] Guid id) => await repository.GetFullPath(id).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpGet]
        public async Task<IActionResult> GetTree([Required] Guid id) => await repository.GetTree(id).ToJsonResultAsync();
    }
}
