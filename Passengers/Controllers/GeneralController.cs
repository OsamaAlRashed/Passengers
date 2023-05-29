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
        public async Task<IActionResult> Users(UserType? type) => await accountRepository.Users(type).ToJsonResultAsync();

        [HttpGet]
        public IActionResult AccountStatus() => Ok(Helpers.EnumToList(typeof(AccountStatus)));

        [HttpGet]
        public IActionResult AddressTypes() => Ok(Helpers.EnumToList(typeof(AddressType)));

        [HttpGet]
        public IActionResult BloodTypes() => Ok(Helpers.EnumToList(typeof(BloodType)));

        [HttpGet]
        public IActionResult ContactShopTypes() => Ok(Helpers.EnumToList(typeof(ContactShopType)));

        [HttpGet]
        public IActionResult DocumentEntityTypes() => Ok(Helpers.EnumToList(typeof(DocumentEntityType)));

        [HttpGet]
        public IActionResult DocumentTypes() => Ok(Helpers.EnumToList(typeof(DocumentType)));

        [HttpGet]
        public IActionResult GenderTypes() => Ok(Helpers.EnumToList(typeof(GenderType)));

        [HttpGet]
        public IActionResult OfferTypes() => Ok(Helpers.EnumToList(typeof(OfferType)));

        [HttpGet]
        public IActionResult OrderStatus() => Ok(Helpers.EnumToList(typeof(OrderStatus)));

        [HttpGet]
        public IActionResult OrderTypes() => Ok(Helpers.EnumToList(typeof(OrderType)));

        [HttpGet]
        public IActionResult ShopOrderType() => Ok(Helpers.EnumToList(typeof(ShopOrderType)));

        [HttpGet]
        public IActionResult SortProductTypes() => Ok(Helpers.EnumToList(typeof(SortProductType)));

        [HttpGet]
        public IActionResult TagTypes() => Ok(Helpers.EnumToList(typeof(TagType)));

        [HttpGet]
        public IActionResult TokenTypes() => Ok(Helpers.EnumToList(typeof(TokenType)));

        [HttpGet]
        public IActionResult UserTypes() => Ok(Helpers.EnumToList(typeof(UserType)));

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
