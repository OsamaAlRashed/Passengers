using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.TagDtos;
using Passengers.Main.TagService;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
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

        [HttpGet]
        public async Task<IActionResult> Get(bool isHidden) => await repository.Get(isHidden).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required]Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetByShopId([Required]Guid shopId) => await repository.GetByShopId(shopId).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetPublicTag() => await repository.GetPublicTag().ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] SetTagDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] SetTagDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Remove(id).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Archive([Required]Guid id) => await repository.ChangeStatus(id, true).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Restore([Required] Guid id) => await repository.ChangeStatus(id, false).ToJsonResultAsync();
    }
}
