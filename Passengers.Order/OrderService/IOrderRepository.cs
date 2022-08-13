using Passengers.DataTransferObject.OrderDtos;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Order.OrderService
{
    public interface IOrderRepository
    {
        Task<OperationResult<List<ResponseCardDto>>> GetMyCart(RequestCardDto dto);
        Task<OperationResult<ResponseAddOrderDto>> AddOrder(SetOrderDto dto, Guid? currentUserId = null);
        Task<OperationResult<ExpectedCostDto>> GetExpectedCost(Guid addressId);
        Task<OperationResult<bool>> ChangeStatus(Guid orderId, OrderStatus newStatus, AppUser? appUser = null, decimal deliveryCost = 0, int expectedTime = 0, string reasonRefuse = "");
        Task<OperationResult<DriverOrderDetailsDto>> Assign(Guid orderId, OrderStatus newStatus);
        Task<OperationResult<OrderDetailsDto>> GetOrderDetails(Guid orderId);
        Task<OperationResult<bool>> OrderReady(Guid orderId);
        Task<OperationResult<List<ShopOrderDto>>> GetShopOrders(bool? isReady, string search);
        Task<OperationResult<List<CustomerOrderDto>>> GetCustomerOrders();
        Task<OperationResult<CustomerOrderDto>> GetCustomerOrderById(Guid orderId);
        Task<OperationResult<string>> Test();
        Task<OperationResult<string>> Test2();
        Task<OperationResult<object>> NextStep(Guid? orderId, Guid? shopId, Guid? customerId, Guid? driverId);

        Task<OperationResult<List<DashboardOrderDto>>> GetOrdersBoard(string search);
        Task<OperationResult<OrderDashboardDetails>> GetOrderDashboardDetails(Guid orderId);

        Task<OperationResult<List<DriverOrderDto>>> GetAvilableOrders();
        Task<OperationResult<DriverOrderDetailsDto>> GetCurrentOrder(Guid? id);
        Task<OperationResult<bool>> DeleteAll();
        Task<OperationResult<bool>> Delete(Guid id);
        Task<OperationResult<bool>> RefuseDriverOrder(Guid orderId);

    }
}
