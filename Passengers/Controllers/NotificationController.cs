using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.Shared.NotificationService;
using Passengers.Security.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Swagger.ApiGroup;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository repository;

        public NotificationController(INotificationRepository repository)
        {
            this.repository = repository;
        }

        [AppAuthorize(AppRoles.Customer, AppRoles.Shop, AppRoles.Driver, AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Customer, ApiGroupNames.Shop, ApiGroupNames.Driver, ApiGroupNames.Dashboard)]
        [HttpGet]
        public async Task<IActionResult> Get() => await repository.Get().ToJsonResultAsync();

    }
}
