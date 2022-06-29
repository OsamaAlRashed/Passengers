using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.Security.AccountService;
using Passengers.Security.AdminService;
using Passengers.SharedKernel.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Swagger.ApiGroup;
using System;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Dashboard)]
    [AppAuthorize(AppRoles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository repository;
        private readonly IAccountRepository accountRepository;

        public AdminController(IAdminRepository repository, IAccountRepository accountRepository)
        {
            this.repository = repository;
            this.accountRepository = accountRepository;
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
        public async Task<IActionResult> Block(Guid id) => await accountRepository.Block(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id) => await accountRepository.Delete(id).ToJsonResultAsync();
    }
}
