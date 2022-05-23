using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.DriverDtos;
using Passengers.Security.AccountService;
using Passengers.Security.AdminService;
using Passengers.Security.DriveService;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using System;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverRepository repository;
        private readonly IAccountRepository accountRepository;

        public DriverController(IDriverRepository repository, IAccountRepository accountRepository)
        {
            this.repository = repository;
            this.accountRepository = accountRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string search, bool? online, int pageNumber = 1, int pageSize = 10) => await repository.Get(pageNumber, pageSize, search, online).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, DateTime? day) => await repository.Details(id, day).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Add(SetDriverDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Update(SetDriverDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Block(Guid id) => await accountRepository.Block(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id) => await accountRepository.Delete(id).ToJsonResultAsync();
    }
}
