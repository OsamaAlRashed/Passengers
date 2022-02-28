using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetEnums() => Ok(new []
        {
            "AccountStatus",
            "BloodTypes",
            "CategoryTypes",
            "ContactShopTypes",
            "DocumentEntityTypes",
            "DocumentTypes"
        });

        [HttpGet]
        public async Task<IActionResult> GetEnumList(string enumName) => Ok();

    }
}
