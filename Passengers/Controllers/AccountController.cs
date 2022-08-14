using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.Security.AccountService;
using Passengers.Security.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Swagger.ApiGroup;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository repository;

        public AccountController(IAccountRepository repository)
        {
            this.repository = repository;
        }

        [ApiGroup(ApiGroupNames.Test)]
        [AppAuthorize(AppRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> SignUp(CreateAccountDto dto) => await repository.Create(dto).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Dashboard)]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(BaseLoginDto dto) => await repository.Login(dto).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Test)]
        [AppAuthorize(AppRoles.Admin)]
        [HttpPut]
        public async Task<IActionResult> ChangePassword(Guid id, string newPassword) => await repository.ChangePassword(id, newPassword).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop, ApiGroupNames.Dashboard, ApiGroupNames.Customer, ApiGroupNames.Driver)]
        [AppAuthorize(AppRoles.Shop, AppRoles.Admin, AppRoles.Customer, AppRoles.Driver)]
        [HttpGet]
        public async Task<IActionResult> Logout(string refreshToken) => await repository.Logout(refreshToken).ToJsonResultAsync();


        [HttpPost]
        [ApiGroup(ApiGroupNames.Test)]
        [AppAuthorize(AppRoles.Admin)]
        public async Task<IActionResult> ChangeStatus([Required]Guid id, [Required]AccountStatus accountStatus) => await repository.ChangeStatus(id, accountStatus).ToJsonResultAsync();

        [HttpPost]
        [AllowAnonymous]
        [ApiGroup(ApiGroupNames.Dashboard, ApiGroupNames.Shop, ApiGroupNames.Customer, ApiGroupNames.Driver)]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            Request.Headers.TryGetValue("Authorization", out var accessToken);
            if (string.IsNullOrEmpty(accessToken) || accessToken.ToString().Split(" ").Length != 2)
                return NotFound("HeaderNotContainAccessToken");
            return await repository.RefreshToken(accessToken.ToString().Split(" ")[1], refreshToken).ToJsonResultAsync();
        }
    }
}
