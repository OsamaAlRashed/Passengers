using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.OrderDtos;
using Passengers.Order.OrderService;
using Passengers.SharedKernel.Attribute;
using Passengers.SharedKernel.Constants.Security;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Swagger.ApiGroup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Passengers.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository repository;

        public OrderController(IOrderRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(SetOrderDto dto) => await repository.AddOrder(dto).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> GetMyCart(RequestCardDto cart) => await repository.GetMyCart(cart).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(Guid orderId) => await repository.GetOrderDetails(orderId).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpGet]
        public async Task<IActionResult> GetShopOrders(bool? isReady, string search) => await repository.GetShopOrders(isReady, search).ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetCustomerOrders() => await repository.GetCustomerOrders().ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetCustomerOrderById([Required]Guid id) => await repository.GetCustomerOrderById(id).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Shop)]
        [HttpPatch]
        public async Task<IActionResult> Ready([Required] Guid orderId) => await repository.OrderReady(orderId).ToJsonResultAsync();

        [HttpPatch]
        public async Task<IActionResult> Cancel([Required] Guid orderId) => await repository.ChangeStatus(orderId, OrderStatus.Canceled).ToJsonResultAsync();

        [HttpPatch]
        public async Task<IActionResult> Refuse([Required] Guid orderId) => await repository.ChangeStatus(orderId, OrderStatus.Refused).ToJsonResultAsync();

        [HttpPatch]
        public async Task<IActionResult> Accept([Required] Guid orderId) => await repository.ChangeStatus(orderId, OrderStatus.Accepted).ToJsonResultAsync();

        [HttpPatch]
        public async Task<IActionResult> Assign([Required] Guid orderId) => await repository.ChangeStatus(orderId, OrderStatus.Assigned).ToJsonResultAsync();

        [HttpPatch]
        public async Task<IActionResult> Collect([Required] Guid orderId) => await repository.ChangeStatus(orderId, OrderStatus.Collected).ToJsonResultAsync();

        [HttpPatch]
        public async Task<IActionResult> Complete([Required] Guid orderId) => await repository.ChangeStatus(orderId, OrderStatus.Completed).ToJsonResultAsync();

    }
}
