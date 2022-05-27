using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.TagDtos;
using Passengers.Main.TagService;
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
    public class TagController : ControllerBase
    {
        private readonly ITagRepository repository;

        public TagController(ITagRepository repository)
        {
            this.repository = repository;
        }


        [ApiGroup(ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Get(bool isHidden) => await repository.Get(isHidden).ToJsonResultAsync();


        [ApiGroup(ApiGroupNames.Shop, ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Shop, AppRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetById([Required]Guid id) => await repository.GetById(id).ToJsonResultAsync();


        [HttpGet]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Admin)]
        public async Task<IActionResult> GetByShopId([Required]Guid shopId) => await repository.GetByShopId(shopId).ToJsonResultAsync();


        [ApiGroup(ApiGroupNames.Shop, ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Shop, AppRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetPublicTag() => await repository.GetPublicTag().ToJsonResultAsync();


        [ApiGroup(ApiGroupNames.Shop, ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Shop, AppRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] SetTagDto dto) => await repository.Add(dto).ToJsonResultAsync();


        [ApiGroup(ApiGroupNames.Shop, ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Shop, AppRoles.Admin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] SetTagDto dto) => await repository.Update(dto).ToJsonResultAsync();


        [ApiGroup(ApiGroupNames.Shop, ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Shop, AppRoles.Admin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Remove(id).ToJsonResultAsync();


        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpPut]
        public async Task<IActionResult> Archive([Required]Guid id) => await repository.ChangeStatus(id, true).ToJsonResultAsync();


        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpPut]
        public async Task<IActionResult> Restore([Required] Guid id) => await repository.ChangeStatus(id, false).ToJsonResultAsync();


        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpGet]
        public async Task<IActionResult> GetTags() => await repository.GetByShopId(null).ToJsonResultAsync();

    }
}
