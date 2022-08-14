using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passengers.DataTransferObject.OrderDtos;
using Passengers.Order.OrderService;
using Passengers.Security.Attribute;
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

        [AppAuthorize(AppRoles.Customer)]
        [ApiGroup(ApiGroupNames.Customer)]
        [HttpPost]
        public async Task<IActionResult> AddOrder(SetOrderDto dto) => await repository.AddOrder(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [ApiGroup(ApiGroupNames.Customer)]
        [HttpPost]
        public async Task<IActionResult> Checkout(SetOrderDto dto) => await repository.Checkout(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer, AppRoles.Shop, AppRoles.Driver, AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Customer, ApiGroupNames.Shop, ApiGroupNames.Driver, ApiGroupNames.Dashboard)]
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails([Required] Guid orderId) => await repository.GetOrderDetails(orderId).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Shop)]
        [ApiGroup(ApiGroupNames.Shop)] 
        [HttpGet]
        public async Task<IActionResult> GetShopOrders(bool? isReady, string search) => await repository.GetShopOrders(isReady, search).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [ApiGroup(ApiGroupNames.Customer)]
        [HttpGet]
        public async Task<IActionResult> GetCustomerOrders() => await repository.GetCustomerOrders().ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [ApiGroup(ApiGroupNames.Customer)]
        [HttpGet]
        public async Task<IActionResult> GetCustomerOrderById([Required]Guid id) => await repository.GetCustomerOrderById(id).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Shop)]
        [ApiGroup(ApiGroupNames.Shop)]
        [HttpPatch]
        public async Task<IActionResult> Ready([Required] Guid orderId) => await repository.OrderReady(orderId).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [ApiGroup(ApiGroupNames.Customer)]
        [HttpPatch]
        public async Task<IActionResult> Cancel([Required] Guid orderId) => await repository.ChangeStatus(orderId, OrderStatus.Canceled).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpPut]
        public async Task<IActionResult> Refuse([Required] Guid orderId, string reasonRefuse) => await repository.ChangeStatus(orderId, OrderStatus.Refused, null, 0,0, reasonRefuse).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpPut]
        public async Task<IActionResult> Accept([Required] Guid orderId, decimal deliveryCost, int expectedTime) => await repository.ChangeStatus(orderId, OrderStatus.Accepted, null, deliveryCost, expectedTime).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Driver, AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Driver, ApiGroupNames.Dashboard)]
        [HttpPatch]
        public async Task<IActionResult> Assign([Required] Guid orderId) => await repository.Assign(orderId, OrderStatus.Assigned).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Driver)]
        [ApiGroup(ApiGroupNames.Driver)]
        [HttpPatch]
        public async Task<IActionResult> Refuse([Required] Guid orderId) => await repository.RefuseDriverOrder(orderId).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Driver)]
        [ApiGroup(ApiGroupNames.Driver)]
        [HttpPatch]
        public async Task<IActionResult> Collect([Required] Guid orderId) => await repository.ChangeStatus(orderId, OrderStatus.Collected).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Driver)]
        [ApiGroup(ApiGroupNames.Driver)]
        [HttpPatch]
        public async Task<IActionResult> Complete([Required] Guid orderId) => await repository.ChangeStatus(orderId, OrderStatus.Completed).ToJsonResultAsync();


        [ApiGroup(ApiGroupNames.Test)]
        [HttpGet]
        public async Task<IActionResult> Test() => await repository.Test().ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Test)]
        [HttpGet]
        public async Task<IActionResult> Test2() => await repository.Test2().ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Test)]
        [HttpGet]
        public async Task<IActionResult> NextStep(Guid? orderId, Guid? shopId, Guid? customerId, Guid? driverId) 
            => await repository.NextStep(orderId, shopId, customerId, driverId).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetOrdersBoard(string search) => await repository.GetOrdersBoard(search).ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Dashboard)]
        [AppAuthorize(AppRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetOrderDashboardDetails([Required] Guid id) => await repository.GetOrderDashboardDetails(id).ToJsonResultAsync();


        [ApiGroup(ApiGroupNames.Driver)]
        [AppAuthorize(AppRoles.Driver)]
        [HttpGet]
        public async Task<IActionResult> GetAvilableOrders() => await repository.GetAvilableOrders().ToJsonResultAsync();

        [ApiGroup(ApiGroupNames.Driver)]
        [AppAuthorize(AppRoles.Driver)]
        [HttpGet]
        public async Task<IActionResult> GetCurrentOrder(Guid? id) => await repository.GetCurrentOrder(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> DeleteAll() => await repository.DeleteAll().ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id) => await repository.Delete(id).ToJsonResultAsync();
    }
}
