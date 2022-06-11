using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.OrderDtos;
using Passengers.Models.Order;
using Passengers.Models.Security;
using Passengers.Order.RealTime;
using Passengers.Order.RealTime.Hubs;
using Passengers.Repository.Base;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderSet = Passengers.Models.Order.Order;

namespace Passengers.Order.OrderService
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        private readonly IHubContext<OrderHub, IOrderHub> orderHubContext;
        private readonly IUserConnectionManager userConnectionManager;

        public OrderRepository(PassengersDbContext context, IHubContext<OrderHub, IOrderHub> orderHubContext, IUserConnectionManager userConnectionManager) : base(context)
        {
            this.orderHubContext = orderHubContext;
            this.userConnectionManager = userConnectionManager;
        }

        public async Task<OperationResult<bool>> AddOrder(SetOrderDto dto)
        {
            if (dto.Cart == null || !dto.Cart.Any())
                return _Operation.SetFailed<bool>("CartNotContainsItems");

            var orders = new List<OrderSet>();
            foreach (var shop in dto.Cart)
            {
                var order = new OrderSet
                {
                    SerialNumber = GenerateSerialNumber(OrderTypes.Instant),
                    DriverNote = dto.DriverNote,
                    AddressId = dto.AddressId,
                    OrderStatusLogs = new List<OrderStatusLog>()
                    {
                        new OrderStatusLog() { Status = OrderStatus.Sended }
                    },
                    OrderType = OrderTypes.Instant,
                    ShopNote = shop.Note,
                    OrderDetails = shop.Products.Select(x => new OrderDetails
                    {
                        ProductId = x.Id,
                        Quantity = x.Count,
                    }).ToList(),
                };
                orders.Add(order);
            }
            Context.Orders.AddRange(orders);
            await Context.SaveChangesAsync();

            var connections = userConnectionManager.GetConnections(UserTypes.Admin);
            foreach (var connection in connections)
            {
                await orderHubContext.Clients.Client(connection).NewOrder(orders.First().SerialNumber);
            }

            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> ChangeStatus(Guid orderId, OrderStatus newStatus)
        {
            var currentUser = await Context.Users.FindAsync(Context.CurrentUserId);
            if (currentUser == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "UserNotFound");

            var order = await Context.Orders.Include(x => x.OrderDetails).Include(x => x.OrderStatusLogs)
                .Where(x => x.Id == orderId).SingleOrDefaultAsync();
            if (order == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "OrderNotFound");

            if(ValidAction(order, currentUser, newStatus))
            {
                Context.OrderStatusLogs.Add(new OrderStatusLog
                {
                    Status = newStatus
                });
                await Context.SaveChangesAsync();

                //Notify(currentUser.UserType, newStatus, order.Address.CustomerId.Value, order.OrderDetails.Select(x => x.Product.Tag.ShopId.Value).FirstOrDefault(), order.DriverId);

                return _Operation.SetSuccess(true);
            }


            return _Operation.SetFailed<bool>("StatusNotValid");
        }

        public async Task<OperationResult<List<ExpectedCostDto>>> GetExpectedCost(Guid addressId, List<Guid> shopIds)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<List<ResponseCardDto>>> GetMyCart(RequestCardDto dto)
        {
            if (dto.Products == null || !dto.Products.Any())
                return _Operation.SetFailed<List<ResponseCardDto>>("");

            var result = (await Context.Products
                .Include(x => x.Tag).ThenInclude(x => x.Shop)
               .Where(x => dto.Products.Select(x => x.Id).Contains(x.Id) && x.Tag.ShopId.HasValue)
               .ToListAsync())
               .GroupBy(x => x.Tag.Shop)
               .Select(x => new ResponseCardDto
               {
                   Id = x.Key.Id,
                   Name = x.Key.Name,
                   Note = dto.Shops == null ? "" : dto.Shops.Where(s => s.Id == x.Key.Id).Select(s => s.Note).FirstOrDefault(),
                   Products = x.Select(x => new ProductCardDto
                   {
                       Id = x.Id,
                       Name = x.Name,
                       Price = x.Price,
                       Count = dto.Products.Where(p => p.Id == x.Id).Select(x => x.Count).FirstOrDefault()
                   }).ToList()
               }).ToList();

            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<OrderDetailsDto>> GetOrderDetails(Guid orderId)
        {
            var order = await Context.Orders.Include(x => x.OrderDetails).ThenInclude(x => x.Product)
                .Where(x => x.Id == orderId).SingleOrDefaultAsync();
            if (order == null)
                return _Operation.SetContent<OrderDetailsDto>(OperationResultTypes.NotExist, "OrderNotFound");

            var result = new OrderDetailsDto
            {
                Products = order.OrderDetails.Select(x => new ProductCardDto
                {
                    Id = x.Product.Id,
                    Name = x.Product.Name,
                    Count = x.Quantity,
                    Price = x.Product.Price
                }).ToList(),
                DeliveryCost = order.DeliveryCost ?? 0,
                Note = order.ShopNote,
                SubTotal = order.OrderDetails.Sum(x => x.Product.Price * x.Quantity),
            };
            result.TotalCost = result.DeliveryCost + result.SubTotal;
            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<bool>> OrderReady(Guid orderId)
        {
            var order = await Context.Orders
                .Where(x => x.Id == orderId).SingleOrDefaultAsync();
            if (order == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "OrderNotFound");
            order.IsShopReady = true;
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<List<ShopOrderDto>>> GetShopOrders(bool? isReady, string search)
        {
            var orders = await Context.Orders
                .Where(x => x.OrderDetails.Select(x => x.Product.Tag.ShopId).Any(id => id == Context.CurrentUserId)
                    && (!isReady.HasValue || x.IsShopReady == isReady) 
                    && (string.IsNullOrEmpty(search) || x.SerialNumber.Contains(search) || x.OrderDetails.Sum(x => x.Quantity * x.Product.Price).ToString().Contains(search)))
                .Select(x => new ShopOrderDto
                {
                    Id = x.Id,
                    SerialNumber = x.SerialNumber,
                    DateCreated = x.DateCreated,
                    Products = x.OrderDetails.Select(x => new ProductCardDto
                    {
                        Id = x.Product.Id,
                        Name = x.Product.Name,
                        Count = x.Quantity,
                        Price = x.Product.Price
                    }).ToList(),
                    TotalPrice = x.OrderDetails.Sum(x => x.Quantity * x.Product.Price)
                }).ToListAsync();

            return _Operation.SetSuccess(orders);
        }

        public async Task<OperationResult<object>> GetCustomerOrders()
        {
            var customer = await Context.Customers()
                .Where(x => x.Id == Context.CurrentUserId).SingleOrDefaultAsync();
            if (customer == null)
                return _Operation.SetContent<object>(OperationResultTypes.NotExist, "");

            var orders = await Context.Orders
                    //.Where(order => customer.Addresses.Any(x => x.Id == order.AddressId))
                    .Select(x => new
                    {
                        x.Id,
                        x.SerialNumber,
                        x.DateCreated,
                        Status = OrderStatusHelper.MapCustomer(x.Status),
                    }).ToListAsync();

            return _Operation.SetSuccess<object>(orders);
        }

        public async Task<OperationResult<CustomerOrderDto>> GetCustomerOrderById(Guid orderId)
        {
            var customer = await Context.Customers().Include(x => x.Addresses)
                .Where(x => x.Id == Context.CurrentUserId).SingleOrDefaultAsync();
            if (customer == null)
                return _Operation.SetContent<CustomerOrderDto>(OperationResultTypes.NotExist, "");

            var order = await Context.Orders
                    .Where(order => order.Id == orderId)
                    .Select(x => new CustomerOrderDto
                    {
                        Id = x.Id,
                        SerialNumber = x.SerialNumber,
                        DateCreated = x.DateCreated,
                        ShopName = x.OrderDetails.Select(x => x.Product.Tag.Shop.Name).FirstOrDefault(),
                        Status = OrderStatusHelper.MapCustomer(x.Status),
                        SubTotal = x.OrderDetails.Sum(x => x.Product.Price * x.Quantity),
                        DeliveryCost = x.DeliveryCost.Value,
                        TotalCost = x.OrderDetails.Sum(x => x.Product.Price * x.Quantity) + (x.DeliveryCost ?? 0),
                        DriverNote = x.DriverNote,
                        AddressTitle = x.Address.Title,
                        ///TODO
                        Distance = 100,
                        Time = 10
                    })
                    .SingleOrDefaultAsync();
       
            return _Operation.SetSuccess(order);
        }


        #region Helpers
        private static string GenerateSerialNumber(OrderTypes type)
            => (type == OrderTypes.Instant ? "A" : "B") + Helpers.GetNumberToken(5);

        private static bool ValidAction(OrderSet order, AppUser currentUser, OrderStatus newStatus)
            => CanCustomerCancel(order, currentUser, newStatus) || CanAdminAction(order, currentUser, newStatus)
            || CanDriverAction(order, currentUser, newStatus);

        // Cancel Order When Status is Draft
        private static bool CanCustomerCancel(OrderSet order, AppUser currentUser, OrderStatus newStatus)
            => currentUser.UserType == UserTypes.Customer && order.Status == OrderStatus.Sended && newStatus == OrderStatus.Canceled;

        // Accepet or refuse order When Status is Draft
        // Assign order when status is Accepted.
        private static bool CanAdminAction(OrderSet order, AppUser currentUser, OrderStatus newStatus)
            => currentUser.UserType == UserTypes.Admin 
                && (
                    ((order.Status == OrderStatus.Sended) && (newStatus == OrderStatus.Accepted || newStatus == OrderStatus.Refused))
                  || (order.Status == OrderStatus.Accepted && newStatus == OrderStatus.Assigned)
                );

        // assign order if sttaus accepted
        // collect order if sttaus accepted
        // assign order if sttaus accepted
        private static bool CanDriverAction(OrderSet order, AppUser currentUser, OrderStatus newStatus)
            => currentUser.UserType == UserTypes.Driver
                && (
                    (order.Status == OrderStatus.Accepted && newStatus == OrderStatus.Assigned)
                 || (order.Status == OrderStatus.Assigned && newStatus == OrderStatus.Collected)
                 || (order.Status == OrderStatus.Collected && newStatus == OrderStatus.Completed)
                );

        private async Task GetConnections(Guid orderId, UserTypes userType, OrderStatus newStatus, Guid customerId, Guid shopId, Guid? driverId)
        {
            var connections = new List<string>();
            
            if(newStatus == OrderStatus.Canceled)
            {
                //Admins
            }
            else if(newStatus == OrderStatus.Accepted)
            {
                //Shop
                connections.AddRange(userConnectionManager.GetConnections(shopId));
                //Customer
                connections.AddRange(userConnectionManager.GetConnections(customerId));
                //Drivers
                connections.AddRange(userConnectionManager.GetConnections(GetAvilableDriverIds()));
            }
            else if(newStatus == OrderStatus.Refused)
            {
                //Customer
                connections.AddRange(userConnectionManager.GetConnections(customerId));
                //Admins
                connections.AddRange(userConnectionManager.GetConnections(UserTypes.Admin));
            }
            else if(newStatus == OrderStatus.Assigned)
            {
                //Driver
                connections.AddRange(userConnectionManager.GetConnections(driverId.Value));
                //Admins
                connections.AddRange(userConnectionManager.GetConnections(UserTypes.Admin));
            }
            if (newStatus == OrderStatus.Assigned)
            {
                //Admins
                connections.AddRange(userConnectionManager.GetConnections(UserTypes.Admin));
            }
            else if(newStatus == OrderStatus.Collected)
            {
                //Admins
                connections.AddRange(userConnectionManager.GetConnections(UserTypes.Admin));
                //Customer
                connections.AddRange(userConnectionManager.GetConnections(customerId));
            }
            else if (newStatus == OrderStatus.Completed)
            {
                //Customer
                connections.AddRange(userConnectionManager.GetConnections(customerId));
                //Admins
            }
            
            
            connections.AddRange(userConnectionManager.GetConnections(UserTypes.Admin, new List<Guid> { }));

            foreach (var connection in connections)
            {
                await orderHubContext.Clients.Client(connection).UpdateOrder(orderId, (int)newStatus);
            }
        }


        public static List<Guid> GetAvilableDriverIds()
        {
            return new List<Guid>();
        }
        #endregion
    }
}
