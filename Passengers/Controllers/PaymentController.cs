using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.PaymentDtos;
using Passengers.Main.PaymentService;
using Passengers.Main.SalaryLogService;
using Passengers.Security.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Swagger.ApiGroup;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiGroup(ApiGroupNames.Dashboard)]
    [AppAuthorize(AppRoles.Admin)]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository repository;

        public PaymentController(IPaymentRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int? year, int? month) => await repository.Get(year, month).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetSalaries(int? year, int? month, string search) => await repository.GetSalaries(year, month,search).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetImports(int? year, int? month, string search) => await repository.GetImports(year, month,search).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetExports(int? year, int? month, string search) => await repository.GetExports(year, month,search).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetNameAndSalary([Required] Guid userId) => await repository.GetNameAndSalary(userId).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Add(PaymentDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Import(ImportPaymentDto dto) => await repository.Import(dto).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Export(ExportPaymentDto dto) => await repository.Export(dto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Update(PaymentDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Delete(id).ToJsonResultAsync();
    }
}
