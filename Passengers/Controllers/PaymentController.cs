using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.PaymentDtos;
using Passengers.Main.PaymentService;
using Passengers.Main.SalaryLogService;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository repository;

        public PaymentController(IPaymentRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => await repository.Get().ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id) => await repository.GetById(id).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Add(PaymentDto dto) => await repository.Add(dto).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Update(PaymentDto dto) => await repository.Update(dto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id) => await repository.Delete(id).ToJsonResultAsync();
    }
}
