using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.DataTransferObject.ShopDtos;
using Passengers.Main.TagService;
using Passengers.Security.ShopService;
using Passengers.SharedKernel.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Swagger.ApiGroup;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly IShopRepository repository;

        public ShopController(IShopRepository repository)
        {
            this.repository = repository;
        }

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpPost]
        public async Task<IActionResult> SignUp(CreateShopAccountDto dto) => await repository.SignUp(dto).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpPost]
        public async Task<IActionResult> Login(LoginMobileDto dto) => await repository.Login(dto).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpPost]
        public async Task<IActionResult> CompleteInfo([FromForm] CompleteInfoShopDto dto) => await repository.CompleteInfo(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [HttpGet]
        [ApiGroup(ApiGroupNames.Dashboard)]
        public async Task<IActionResult> Get([Required]AccountStatus accountStatus) => await repository.Get(accountStatus).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpGet]
        public async Task<IActionResult> Details() => await repository.Details().ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpPut]
        public async Task<IActionResult> UpdateImage(IFormFile file) => await repository.UpdateImage(file).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpGet]
        public async Task<IActionResult> GetProfile() => await repository.GetProfile().ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpGet]
        public async Task<IActionResult> Home() => await repository.Home().ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpPut]
        public async Task<IActionResult> Update(ShopDetailsDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpPut]
        public async Task<IActionResult> UpdateWorkingDays(List<int> days) => await repository.UpdateWorkingDays(days).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpPut]
        public async Task<IActionResult> UpdateWorkingTimes(string fromTime, string toTime) => await repository.UpdateWorkingTimes(fromTime, toTime).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [AppAuthorize(AppRoles.Shop)]
        [HttpGet]
        public async Task<IActionResult> GetWorkingDays() => await repository.GetWorkingDays().ToJsonResultAsync();

    }
}
