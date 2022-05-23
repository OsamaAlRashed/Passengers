using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.Security.AccountService;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using System;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly IAccountRepository accountRepository;

        public GeneralController(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Users(UserTypes? type) => await accountRepository.Users(type).ToJsonResultAsync();

        [HttpGet]
        public IActionResult AccountStatus() => Ok(Helpers.EnumToList(typeof(AccountStatus)));

        [HttpGet]
        public IActionResult AddressTypes() => Ok(Helpers.EnumToList(typeof(AddressTypes)));

        [HttpGet]
        public IActionResult BloodTypes() => Ok(Helpers.EnumToList(typeof(BloodTypes)));

        [HttpGet]
        public IActionResult ContactShopTypes() => Ok(Helpers.EnumToList(typeof(ContactShopTypes)));

        [HttpGet]
        public IActionResult DocumentEntityTypes() => Ok(Helpers.EnumToList(typeof(DocumentEntityTypes)));

        [HttpGet]
        public IActionResult DocumentTypes() => Ok(Helpers.EnumToList(typeof(DocumentTypes)));

        [HttpGet]
        public IActionResult GenderTypes() => Ok(Helpers.EnumToList(typeof(GenderTypes)));

        [HttpGet]
        public IActionResult NotesToDrive() => Ok(Helpers.EnumToList(typeof(NotesToDrive)));

        [HttpGet]
        public IActionResult OfferTypes() => Ok(Helpers.EnumToList(typeof(OfferTypes)));

        [HttpGet]
        public IActionResult OrderStatus() => Ok(Helpers.EnumToList(typeof(OrderStatus)));

        [HttpGet]
        public IActionResult OrderTypes() => Ok(Helpers.EnumToList(typeof(OrderTypes)));

        [HttpGet]
        public IActionResult ShopOrderType() => Ok(Helpers.EnumToList(typeof(ShopOrderType)));

        [HttpGet]
        public IActionResult SortProductTypes() => Ok(Helpers.EnumToList(typeof(SortProductTypes)));

        [HttpGet]
        public IActionResult TagTypes() => Ok(Helpers.EnumToList(typeof(TagTypes)));

        [HttpGet]
        public IActionResult TokenTypes() => Ok(Helpers.EnumToList(typeof(TokenTypes)));

        [HttpGet]
        public IActionResult UserTypes() => Ok(Helpers.EnumToList(typeof(UserTypes)));

        [HttpGet]
        public IActionResult DayOfWeek() => Ok(Helpers.EnumToList(typeof(DayOfWeek)));

        [HttpGet]
        public IActionResult PaymentTypes() => Ok(Helpers.EnumToList(typeof(PaymentType)));

        [HttpGet]
        public IActionResult ImportTypes() => Ok(Helpers.EnumToList(typeof(ImportType)));

        [HttpGet]
        public IActionResult ExportTypes() => Ok(Helpers.EnumToList(typeof(ExportType)));
    }
}
