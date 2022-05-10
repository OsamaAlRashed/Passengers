using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.Security.AccountService;
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

        [HttpPost]
        public async Task<IActionResult> SignUp(CreateAccountDto dto) => await repository.Create(dto).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Login(BaseLoginDto dto) => await repository.Login(dto).ToJsonResultAsync();

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeStatus([Required]Guid id, [Required]AccountStatus accountStatus) => await repository.ChangeStatus(id, accountStatus).ToJsonResultAsync();

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> ForgetPassword([Required] string email) => await repository.ForgetPassword(email).ToJsonResultAsync();

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResetPassword(ResetPasswordDto dto) => await repository.ResetPassword(dto).ToJsonResultAsync();

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
