using Passengers.DataTransferObject.OrderDtos;
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
        Task<OperationResult<bool>> AddOrder(SetOrderDto dto);
        Task<OperationResult<List<ExpectedCostDto>>> GetExpectedCost(Guid addressId, List<Guid> shopIds);
        Task<OperationResult<bool>> ChangeStatus(Guid orderId, OrderStatus newStatus);
        Task<OperationResult<OrderDetailsDto>> GetOrderDetails(Guid orderId);
        Task<OperationResult<bool>> OrderReady(Guid orderId);
        Task<OperationResult<List<ShopOrderDto>>> GetShopOrders(bool? isReady, string search);
        Task<OperationResult<object>> GetCustomerOrders();
        Task<OperationResult<CustomerOrderDto>> GetCustomerOrderById(Guid orderId);
    }
}
