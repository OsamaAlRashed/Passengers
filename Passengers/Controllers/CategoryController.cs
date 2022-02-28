using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.SharedDtos.CategoryDtos;
using Passengers.Shared.CategoryService;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
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

        [HttpGet]
        public async Task<IActionResult> Get() => await repository.Get().ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required]Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] SetCategoryDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] SetCategoryDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Remove(id).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetRoots() => await repository.GetRoots().ToJsonResultAsync();
        [HttpGet]
        public async Task<IActionResult> GetChildern([Required] Guid id) => await repository.GetChildern(id).ToJsonResultAsync();
        [HttpGet]
        public async Task<IActionResult> GetFullPath([Required] Guid id) => await repository.GetFullPath(id).ToJsonResultAsync();
        [HttpGet]
        public async Task<IActionResult> GetTree([Required] Guid id) => await repository.GetTree(id).ToJsonResultAsync();
    }
}
