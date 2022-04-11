using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.Security.AdminService;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using System;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository repository;

        public AdminController(IAdminRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetShops(int pageNumber = 1, int pageSize = 10, string search = "") => await repository.GetShops(pageNumber, pageSize, search).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber = 1, int pageSize = 10, string search = "") => await repository.Get(pageNumber, pageSize, search).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Add(SetAdminDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Update(SetAdminDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Block(Guid id) => await repository.Block(id).ToJsonResultAsync();


    }
}
