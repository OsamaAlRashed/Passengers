using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.OrderDtos;
using Passengers.Models.Order;
using Passengers.Models.Security;
using Passengers.Order.RealTime;
using Passengers.Order.RealTime.Hubs;
using Passengers.Repository.Base;
using Passengers.Security.AccountService;
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
        private readonly IAccountRepository accountRepository;

        public OrderRepository(PassengersDbContext context, IHubContext<OrderHub, IOrderHub> orderHubContext, IUserConnectionManager userConnectionManager, IAccountRepository accountRepository) : base(context)
        {
            this.orderHubContext = orderHubContext;
            this.userConnectionManager = userConnectionManager;
            this.accountRepository = accountRepository;
        }

        public async Task<OperationResult<ResponseAddOrderDto>> AddOrder(SetOrderDto dto)
        {
            if (dto.Cart == null || !dto.Cart.Any())
                return _Operation.SetFailed<ResponseAddOrderDto>("CartNotContainsItems");

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

            await _UpdateOrdersListCustomer(Context.CurrentUserId.Value);
            await _UpdateOrdersListDashboard();

            var lodedOrders = await Context.Orders
                .Include("OrderDetails.Product.Tag.Shop")
                .Include("OrderDetails.Product.PriceLogs")
                .Where(x => orders.Select(x => x.Id).Contains(x.Id))
                .ToListAsync();

            var result = new ResponseAddOrderDto
            {
                Shops = lodedOrders.Select(x => new ShopCostDto
                {
                    ShopName = x.OrderDetails.Select(x => x.Product.Tag.Shop.Name).FirstOrDefault(),
                    Cost = x.OrderDetails.Sum(x => x.Product.Price * x.Quantity)
                }).ToList()
            };
            result.SubTotal = result.Shops.Sum(x => x.Cost);
            return _Operation.SetSuccess(result);
        }

       

        public async Task<OperationResult<bool>> ChangeStatus(Guid orderId, OrderStatus newStatus)
        {
            var currentUser = await Context.Users.FindAsync(Context.CurrentUserId);
            if (currentUser == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "UserNotFound");

            var order = await Context.Orders
                .Include(x => x.OrderDetails).ThenInclude(x => x.Product).ThenInclude(x => x.Tag)
                .Include(x => x.OrderStatusLogs).Include(x => x.Address)
                .Where(x => x.Id == orderId).SingleOrDefaultAsync();
            if (order == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "OrderNotFound");

            if(ValidAction(order, currentUser, newStatus))
            {
                Context.OrderStatusLogs.Add(new OrderStatusLog
                {
                    Status = newStatus,
                    OrderId = order.Id
                });
                await Context.SaveChangesAsync();

                await Invoke(order, newStatus, order.Address.CustomerId.Value, order.OrderDetails.Select(x => x.Product.Tag.ShopId).FirstOrDefault().Value, order.DriverId);
                return _Operation.SetSuccess(true);
            }


            return _Operation.SetFailed<bool>("StatusNotValid");
        }

        public async Task<OperationResult<ExpectedCostDto>> GetExpectedCost(Guid addressId)
        {
            Random random = new Random();
            var cost = random.Next(20, 50) * 100;
            var time = random.Next(10, 30);
            return _Operation.SetSuccess(new ExpectedCostDto { Cost = cost, Time = time });
        }

        public async Task<OperationResult<List<ResponseCardDto>>> GetMyCart(RequestCardDto dto)
        {
            if (dto.Products == null || !dto.Products.Any())
                return _Operation.SetFailed<List<ResponseCardDto>>("");

            var result = (await Context.Products
                .Include(x => x.Tag).ThenInclude(x => x.Shop).Include(x => x.PriceLogs)
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
            var orders = await _GetShopOrders(isReady, search);

            return _Operation.SetSuccess(orders);
        }

        private async Task<List<ShopOrderDto>> _GetShopOrders(bool? isReady = null, string search = "")
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

            return orders;
        }

        public async Task<OperationResult<List<CustomerOrderDto>>> GetCustomerOrders()
        {
            var orders = await _GetCustomerOrders();
            if(orders == null)
                _Operation.SetContent<CustomerOrderDto>(OperationResultTypes.NotExist, "");

            return _Operation.SetSuccess(orders);
        }

        private async Task<List<CustomerOrderDto>> _GetCustomerOrders(Guid? id = null)
        {
            id = id.HasValue ? id.Value : Context.CurrentUserId;
            var customer = await Context.Customers()
                .Include(x => x.Addresses)
                .Where(x => x.Id == id).SingleOrDefaultAsync();
            if (customer == null)
                return null;

            var orders = await Context.Orders
                    .Include(x => x.OrderStatusLogs)
                    .Where(order => customer.Addresses.Select(x => x.Id).Contains(order.AddressId))
                    .Select(x => new CustomerOrderDto
                    {
                        Id = x.Id,
                        SerialNumber = x.SerialNumber,
                        DateCreated = x.DateCreated,
                        Status = OrderStatusHelper.MapCustomer(x.Status),
                    }).ToListAsync();

            orders = orders.Where(x => x.Status <= CustomerOrderStatus.Completed).ToList();
            return orders;
        }

        public async Task<OperationResult<CustomerOrderDto>> GetCustomerOrderById(Guid orderId)
        {
            var customer = await Context.Customers().Include(x => x.Addresses)
                .Where(x => x.Id == Context.CurrentUserId).SingleOrDefaultAsync();
            if (customer == null)
                return _Operation.SetContent<CustomerOrderDto>(OperationResultTypes.NotExist, "");

            var order = await Context.Orders
                    .Where(order => order.Id == orderId)
                    .Include("OrderDetails.Product.Tag.Shop")
                    .Include("OrderDetails.Product.PriceLogs")
                    .Include(x => x.OrderStatusLogs)
                    .Include(x => x.Address)
                    .SingleOrDefaultAsync();

            var result = new CustomerOrderDto
            {
                Id = order.Id,
                SerialNumber = order.SerialNumber,
                DateCreated = order.DateCreated,
                ShopName = order.OrderDetails.Select(x => x.Product.Tag.Shop.Name).FirstOrDefault(),
                Status = OrderStatusHelper.MapCustomer(order.Status),
                SubTotal = order.OrderDetails.Select(x => new
                {
                    Price = x.Product.Price,
                    Quantity = x.Quantity
                }).Sum(x => x.Price * x.Quantity),
                DeliveryCost = order.DeliveryCost ?? 0,
                DriverNote = order.DriverNote,
                AddressTitle = order.Address.Title,
                ///TODO
                Distance = 100,
                Time = 10,
            };

            result.TotalCost = result.SubTotal + (result.DeliveryCost ?? 0);

            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<string>> Test()
        {
            await orderHubContext.Clients.User("a2b0d8d0-4d71-4ea0-95ab-08da0c512705").Test("Hello from Test.");

            return _Operation.SetSuccess<string>("Hello from Test.");
        }

        public async Task<OperationResult<string>> Test2()
        {
            await orderHubContext.Clients.All.Test2("Hello from Test", "2");

            return _Operation.SetSuccess<string>("Hello from Test 2.");
        }

        #region Helpers
        private async Task<bool> _UpdateOrdersListCustomer(Guid id)
        {
            var result = await _GetCustomerOrders(id);
            if (result == null)
                return false;
            await orderHubContext.Clients.User(id.ToString()).UpdateCustomerOrders(result);
            return true;
        }

        private async Task<bool> _UpdateOrdersListShop(Guid id)
        {
            var result = await _GetShopOrders();
            await orderHubContext.Clients.User(id.ToString()).UpdateShopOrders(result);
            return true;
        }

        private async Task<bool> _UpdateOrdersListDashboard()
        {
            ///ToDo
            var admins = accountRepository.GetUserIds(UserTypes.Admin);
            await orderHubContext.Clients.Users(admins.Select(x => x.ToString())).UpdateAdminOrders(new List<DashboardOrderDto>());
            return true;
        }

        private static string GenerateSerialNumber(OrderTypes type)
            => (type == OrderTypes.Instant ? "A" : "B") + Helpers.GetNumberToken(5);

        private static bool ValidAction(OrderSet order, AppUser currentUser, OrderStatus newStatus)
            => CanCustomerCancel(order, currentUser, newStatus) || CanAdminAction(order, currentUser, newStatus)
            || CanDriverAction(order, currentUser, newStatus);

        private static bool CanCustomerCancel(OrderSet order, AppUser currentUser, OrderStatus newStatus)
            => currentUser.UserType == UserTypes.Customer && order.Status == OrderStatus.Sended && newStatus == OrderStatus.Canceled;

        private static bool CanAdminAction(OrderSet order, AppUser currentUser, OrderStatus newStatus)
            => currentUser.UserType == UserTypes.Admin 
                && (
                    ((order.Status == OrderStatus.Sended) && (newStatus == OrderStatus.Accepted || newStatus == OrderStatus.Refused))
                  || (order.Status == OrderStatus.Accepted && newStatus == OrderStatus.Assigned)
                );

        private static bool CanDriverAction(OrderSet order, AppUser currentUser, OrderStatus newStatus)
            => currentUser.UserType == UserTypes.Driver
                && (
                    (order.Status == OrderStatus.Accepted && newStatus == OrderStatus.Assigned)
                 || (order.Status == OrderStatus.Assigned && newStatus == OrderStatus.Collected)
                 || (order.Status == OrderStatus.Collected && newStatus == OrderStatus.Completed)
                );

        private async Task Invoke(OrderSet order, OrderStatus newStatus, Guid customerId, Guid shopId, Guid? driverId)
        {
            await _UpdateOrdersListDashboard();

            if (newStatus == OrderStatus.Canceled)
            {
                await _UpdateOrdersListCustomer(customerId);
            }
            else if(newStatus == OrderStatus.Accepted)
            {
                await _UpdateOrdersListShop(shopId);
                await _UpdateOrdersListCustomer(customerId);

                //Drivers
                var drivers = (await GetAvilableDriverIds(order.Address.Lat, order.Address.Long, order.DeliveryCost ?? 0)).Where(id => id != driverId.Value).ToList();
            }
            else if(newStatus == OrderStatus.Refused)
            {
                await _UpdateOrdersListCustomer(customerId);
            }
            else if(newStatus == OrderStatus.Assigned)
            {

                ///TODO
                var drivers = (await GetAvilableDriverIds(order.Address.Lat, order.Address.Long, order.DeliveryCost ?? 0)).Where(id => id != driverId.Value).ToList();
                var driversConnections = userConnectionManager.GetConnections(drivers);
            }
            else if(newStatus == OrderStatus.Collected)
            {
                await _UpdateOrdersListCustomer(customerId);
            }
            else if (newStatus == OrderStatus.Completed)
            {
                await _UpdateOrdersListCustomer(customerId);
            }
        }

        private async Task<List<Guid>> GetAvilableDriverIds(string latitude, string longitude, decimal total)
        {

            //Nearby Drivers ;
            //Avilable
            //Online
            //Fixed Amount

            var notAvilableDrivers = (await Context.Orders.ToListAsync()).Where(x => x.Status >= OrderStatus.Assigned && x.Status <= OrderStatus.Accepted)
                .Select(x => x.DriverId).ToList();

            var drivers = await Context.Drivers()
                .Where(x => x.DriverOnline.HasValue && x.DriverOnline.Value && !notAvilableDrivers.Contains(x.Id) 
                    && x.Payments.Where(payment => payment.Type.IsFixed())
                                 .Sum(payment => payment.Amount * payment.Type.PaymentSign()) >= total)
                .OrderByDescending(x => CalculateDistance(new ValueTuple<string, string>(latitude, longitude), new ValueTuple<string, string>(x.Address.Lat, x.Address.Long)))
                .Take(5).ToListAsync();
            
            return drivers.Select(x => x.Id).ToList();
        }

        private static double CalculateDistance((string, string) point1, (string, string) point2)
        {
            var latitude1 = double.Parse(point1.Item1);
            var longitude1 = double.Parse(point1.Item2);

            var latitude2 = double.Parse(point2.Item1);
            var longitude2 = double.Parse(point2.Item2);

            var d1 = latitude1 * (Math.PI / 180.0);
            var num1 = longitude1 * (Math.PI / 180.0);
            var d2 = latitude2 * (Math.PI / 180.0);
            var num2 = longitude2 * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                     Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }
        
        #endregion
    }
}
