using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.DriverDtos;
using Passengers.Security.AccountService;
using Passengers.Security.AdminService;
using Passengers.Security.DriveService;
using Passengers.Security.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Swagger.ApiGroup;
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

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpGet]
        public async Task<IActionResult> Get(string search, bool? online, int pageNumber = 1, int pageSize = 10) => await repository.Get(pageNumber, pageSize, search, online).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpGet]
        public async Task<IActionResult> GetById(Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, DateTime? day) => await repository.Details(id, day).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpPost]
        public async Task<IActionResult> Add(SetDriverDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpPut]
        public async Task<IActionResult> Update(SetDriverDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpPut]
        public async Task<IActionResult> Block(Guid id) => await accountRepository.Block(id).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Admin)]
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id) => await accountRepository.Delete(id).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Driver)]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDriverDto dto) => await repository.Login(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Driver)]
        [ApiGroup(ApiGroupNames.Driver)]
        [HttpGet]
        public async Task<IActionResult> GetMyInformations() => await repository.GetMyInformations().ToJsonResultAsync();

        [AppAuthorize(AppRoles.Driver)]
        [ApiGroup(ApiGroupNames.Driver)]
        [HttpPatch]
        public async Task<IActionResult> ChangeAvilability(bool status, string lat, string lng) => await repository.ChangeAvilability(status, lat, lng).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Driver)]
        [ApiGroup(ApiGroupNames.Driver)]
        [HttpPatch]
        public async Task<IActionResult> UploadImage(IFormFile file) => await repository.UploadImage(file).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Driver)]
        [AppAuthorize(AppRoles.Driver)]
        [HttpGet]
        public async Task<IActionResult> GetStatistics(DateTime? date) => await repository.GetStatistics(date).ToJsonResultAsync();

    }
}
