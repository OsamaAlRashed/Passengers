using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Swagger.ApiGroup;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ISharedRepository repository;

        public HomeController(ISharedRepository repository)
        {
            this.repository = repository;
        }

        [ApiGroup(ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetByCustomerId() => await repository.GetHomeDetails().ToJsonResultAsync();

    }
}
