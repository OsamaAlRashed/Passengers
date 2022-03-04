using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.DataTransferObject.SecurityDtos.Login;
using Passengers.DataTransferObject.ShopDtos;
using Passengers.Security.ShopService;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
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

        [HttpPost]
        public async Task<IActionResult> SignUp(CreateShopAccountDto dto) => await repository.SignUp(dto).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Login(LoginMobileDto dto) => await repository.Login(dto).ToJsonResultAsync();

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CompleteInfo(CompleteInfoShopDto dto) => await repository.CompleteInfo(dto).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> Get([Required]AccountStatus accountStatus) => await repository.Get(accountStatus).ToJsonResultAsync();

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details() => await repository.Details().ToJsonResultAsync();

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateImage([FromForm] IFormFile file) => await repository.UpdateImage(file).ToJsonResultAsync();

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProfile() => await repository.GetProfile().ToJsonResultAsync();

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update(ShopDetailsDto dto) => await repository.Update(dto).ToJsonResultAsync();
    }
}
