using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.Shared.SettingService;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Swagger.ApiGroup;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [AppAuthorize(AppRoles.Admin)]
    [ApiGroup(ApiGroupNames.Dashboard)]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISettingRepository repository;

        public SettingController(ISettingRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings() => await repository.GetSettings().ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> SetSettings([Required] decimal price) => await repository.SetSettings(price).ToJsonResultAsync();

    }
}
