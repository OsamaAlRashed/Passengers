﻿using Microsoft.AspNetCore.Http;
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

        [AppAuthorize(AppRoles.Customer)]
        [ApiGroup(ApiGroupNames.Customer)]
        [HttpPost]
        public async Task<IActionResult> AddOrder(SetOrderDto dto) => await repository.AddOrder(dto).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [ApiGroup(ApiGroupNames.Customer)]
        [HttpPost]
        public async Task<IActionResult> GetMyCart(RequestCardDto cart) => await repository.GetMyCart(cart).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Customer)]
        [ApiGroup(ApiGroupNames.Customer)]
        [HttpGet]
        public async Task<IActionResult> GetExpectedCost([Required] Guid addressId) => await repository.GetExpectedCost(addressId).ToJsonResultAsync();

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
        [HttpPatch]
        public async Task<IActionResult> Refuse([Required] Guid orderId) => await repository.ChangeStatus(orderId, OrderStatus.Refused).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Dashboard)]
        [HttpPatch]
        public async Task<IActionResult> Accept([Required] Guid orderId) => await repository.ChangeStatus(orderId, OrderStatus.Accepted).ToJsonResultAsync();

        [AppAuthorize(AppRoles.Driver, AppRoles.Admin)]
        [ApiGroup(ApiGroupNames.Driver, ApiGroupNames.Dashboard)]
        [HttpPatch]
        public async Task<IActionResult> Assign([Required] Guid orderId) => await repository.ChangeStatus(orderId, OrderStatus.Assigned).ToJsonResultAsync();

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

    }
}