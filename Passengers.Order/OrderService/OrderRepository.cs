using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.NotificationDtos;
using Passengers.DataTransferObject.OrderDtos;
using Passengers.Main.ProductService.Store;
using Passengers.Models.Order;
using Passengers.Models.Security;
using Passengers.Order.OrderService.Store;
using Passengers.Order.RealTime;
using Passengers.Order.RealTime.Hubs;
using Passengers.Repository.Base;
using Passengers.Security.AccountService;
using Passengers.Shared.NotificationService;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SharedKernel.Map;
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
        #region Ctor & Props

        private readonly IHubContext<OrderHub, IOrderHub> orderHubContext;
        private readonly IUserConnectionManager userConnectionManager;
        private readonly IAccountRepository accountRepository;
        private readonly INotificationRepository notificationRepository;

        public OrderRepository(PassengersDbContext context, IHubContext<OrderHub, IOrderHub> orderHubContext, IUserConnectionManager userConnectionManager,
            IAccountRepository accountRepository, INotificationRepository notificationRepository) : base(context)
        {
            this.orderHubContext = orderHubContext;
            this.userConnectionManager = userConnectionManager;
            this.accountRepository = accountRepository;
            this.notificationRepository = notificationRepository;
        }

        #endregion

        #region Shared
        public async Task<OperationResult<bool>> ChangeStatus(Guid orderId, OrderStatus newStatus, AppUser? mockCurrentUser = null, decimal deliveryCost = 0, int expectedTime = 0, string reasonRefuse = "")
        {
            var currentUser = await Context.Users.FindAsync(Context.CurrentUserId);
            if (mockCurrentUser != null)
            {
                currentUser = mockCurrentUser;
            }
            if (currentUser == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "UserNotFound");

            var order = await Context.Orders
                .Include(x => x.Driver)
                .Include(x => x.OrderDetails).ThenInclude(x => x.Product).ThenInclude(x => x.Tag).ThenInclude(x => x.Shop)
                .Include(x => x.OrderStatusLogs)
                .Include(x => x.Address).ThenInclude(x => x.Customer)
                .Where(x => x.Id == orderId).SingleOrDefaultAsync();
            if (order == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "OrderNotFound");

            if (ValidAction(order, currentUser, newStatus))
            {
                Context.OrderStatusLogs.Add(new OrderStatusLog
                {
                    Status = newStatus,
                    Note = newStatus == OrderStatus.Refused ? reasonRefuse : null,
                    OrderId = order.Id
                });

                if (newStatus == OrderStatus.Assigned)
                {
                    if(await _DriverAvilable(currentUser.Id))
                    {
                        order.DriverId = currentUser.Id;
                        await _AcceptDriverOrder(orderId, currentUser.Id);
                    }
                    else
                    {
                        return _Operation.SetFailed<bool>("Driver not avilable.");
                    }
                }

                if (newStatus == OrderStatus.Accepted)
                {
                    order.DeliveryCost = deliveryCost;
                    order.ExpectedTime = expectedTime;
                }
                await Context.SaveChangesAsync();

                await Invoke(order, newStatus, order.Address.CustomerId.Value, order.OrderDetails.Select(x => x.Product.Tag.ShopId).FirstOrDefault().Value, order.DriverId);
                return _Operation.SetSuccess(true);
            }

            return _Operation.SetFailed<bool>("StatusNotValid");
        }

        public async Task<OperationResult<OrderDetailsDto>> GetOrderDetails(Guid orderId)
        {
            var order = await Context.Orders.Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product).ThenInclude(x => x.PriceLogs)
                .ThenInclude(x => x.Product).ThenInclude(x => x.Documents)
                .Where(x => x.Id == orderId).SingleOrDefaultAsync();
            if (order == null)
                return _Operation.SetContent<OrderDetailsDto>(OperationResultTypes.NotExist, "OrderNotFound");

            var result = new OrderDetailsDto
            {
                Products = order.OrderDetails.Select(x => new ProductCardDto
                {
                    Id = x.Product.Id,
                    Name = x.Product.Name,
                    ImagePath = x.Product.ImagePath(),
                    Count = x.Quantity,
                    Price = x.Product.Price()
                }).ToList(),
                DeliveryCost = order.DeliveryCost ?? 0,
                Note = order.ShopNote,
                SubTotal = order.Cost(),
            };
            result.TotalCost = result.DeliveryCost + result.SubTotal;
            return _Operation.SetSuccess(result);
        }

        private async Task Invoke(OrderSet order, OrderStatus newStatus, Guid customerId, Guid shopId, Guid? driverId)
        {
            //Admins
            await _UpdateOrdersListDashboard();
            var admins = accountRepository.GetUserIds(UserTypes.Admin);
            await SendNotification(admins, UserTypes.Admin, newStatus, order);

            if (newStatus == OrderStatus.Canceled)
            {
                await _UpdateOrdersListCustomer(customerId);
                await _UpdateOrderCustomer(order.Id);
                await SendNotification(new List<Guid>() { customerId }, UserTypes.Customer, newStatus, order);
            }
            else if (newStatus == OrderStatus.Accepted)
            {
                await _UpdateOrdersListShop(shopId);
                await SendNotification(new List<Guid>() { shopId }, UserTypes.Shop, newStatus, order);

                await _UpdateOrdersListCustomer(customerId);
                await _UpdateOrderCustomer(order.Id);
                await SendNotification(new List<Guid>() { customerId }, UserTypes.Customer, newStatus, order);

                //Drivers
                var driverIds = await GetAvilableDriverIds(order.Id, order.Shop().Address.Lat, order.Shop().Address.Long, order.TotalCost());
                await _SendToDrivers(order.Id, driverIds);
                await _UpdateOrdersListDriver(driverIds);
            }
            else if (newStatus == OrderStatus.Refused)
            {
                await _UpdateOrdersListCustomer(customerId);
                await _UpdateOrderCustomer(order.Id);
                await SendNotification(new List<Guid>() { customerId }, UserTypes.Customer, newStatus, order);
            }
            else if (newStatus == OrderStatus.Assigned)
            {
                var driverIds = await Context.OrderDrivers
                    .Where(x => x.OrderId == order.Id && x.OrderDriverType == OrderDriverType.Nothing)
                    .Select(x => x.DriverId).ToListAsync();
                await _UpdateOrdersListDriver(driverIds);
            }
            else if (newStatus == OrderStatus.Collected)
            {
                await _UpdateOrdersListCustomer(customerId);
                await _UpdateOrderCustomer(order.Id);
                await SendNotification(new List<Guid>() { customerId }, UserTypes.Customer, newStatus, order);
            }
            else if (newStatus == OrderStatus.Completed)
            {
                await _UpdateOrdersListCustomer(customerId);
                await _UpdateOrderCustomer(order.Id);
                await SendNotification(new List<Guid>() { customerId }, UserTypes.Customer, newStatus, order);
            }
        }

        private async Task<NotificationDto> SendNotification(List<Guid> userIds, UserTypes userType, OrderStatus status, OrderSet order)
        {
            var notification = new NotificationDto()
            {
                UserIds = userIds
            };
            switch (userType)
            {
                case UserTypes.Admin:
                    if (status == OrderStatus.Sended)
                    {
                        notification.Title = $"You have a new order {order.SerialNumber} look at it.";
                    }
                    else if (status == OrderStatus.Canceled)
                    {
                        notification.Title = $"The customer {order.Address.Customer.FullName} canceled his order {order.SerialNumber}.";
                    }
                    else if (status == OrderStatus.Assigned)
                    {
                        notification.Title = $"The driver {order.Driver.FullName} accepted order {order.Driver.FullName}";
                    }
                    else if (status == OrderStatus.Completed)
                    {
                        notification.Title = $"The driver {order.Driver.FullName} Deliver order {order.Driver.FullName}";
                    }
                    break;
                case UserTypes.Driver:
                    if (status == OrderStatus.Accepted)
                    {
                        notification.Title = $"You have a new order {order.SerialNumber} look at it";
                    }
                    break;
                case UserTypes.Customer:
                    if (status == OrderStatus.Accepted)
                    {
                        notification.Title = $"Your order {order.SerialNumber} is accepted.";
                        notification.Body = "Your order is cooking wait for it.";
                    }
                    else if (status == OrderStatus.Refused)
                    {
                        notification.Title = $"Your order {order.SerialNumber} is refused.";
                        notification.Body = order.OrderStatusLogs.OrderByDescending(x => x.DateCreated).Select(x => x.Note).FirstOrDefault();
                    }
                    else if (status == OrderStatus.Collected)
                    {
                        notification.Title = $"Your order {order.SerialNumber} is collected.";
                        notification.Body = "Stay tuned, order is caming.";
                    }
                    else if (status == OrderStatus.Completed)
                    {
                        notification.Title = $"Your order {order.SerialNumber} is completed.";
                        notification.Body = "Thanks for using our app, if you have any problems, please send feedback or communicate with customer services.";
                    }
                    break;
                case UserTypes.Shop:
                    if (status == OrderStatus.Accepted)
                    {
                        notification.Title = $"You have a new order {order.SerialNumber}.";
                        notification.Body = $"The driver comes in {order.ExpectedTime} min, prepare order please while he comes.";
                    }
                    break;
            }

            if (notification.Title != null && notification.Body != null)
                return (await notificationRepository.Add(notification)).Result;

            return null;
        }

        #endregion

        #region Customer

        public async Task<OperationResult<List<ResponseCardDto>>> GetMyCart(RequestCardDto dto)
        {
            if (dto.Products == null || !dto.Products.Any())
                return _Operation.SetFailed<List<ResponseCardDto>>("");

            var result = (await Context.Products
                .Include(x => x.Tag).ThenInclude(x => x.Shop).Include(x => x.PriceLogs)
                .Include(x => x.Documents)
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
                       Price = x.Price(),
                       Count = dto.Products.Where(p => p.Id == x.Id).Select(x => x.Count).FirstOrDefault(),
                       ImagePath = x.ImagePath(),
                   }).ToList()
               }).ToList();

            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<ExpectedCostDto>> GetExpectedCost(Guid addressId)
        {
            Random random = new Random();
            var cost = random.Next(20, 50) * 100;
            var time = random.Next(10, 30);
            return _Operation.SetSuccess(new ExpectedCostDto { Cost = cost, Time = time });
        }

        public async Task<OperationResult<ResponseAddOrderDto>> AddOrder(SetOrderDto dto, Guid? currentUserId = null)
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


            if (currentUserId == null)
                currentUserId = Context.CurrentUserId;
            await _UpdateOrdersListCustomer(currentUserId.Value);
            await _UpdateOrdersListDashboard();
            var admins = accountRepository.GetUserIds(UserTypes.Admin);


            var lodedOrders = await Context.Orders
                .Include("OrderDetails.Product.Tag.Shop")
                .Include("OrderDetails.Product.PriceLogs")
                .Where(x => orders.Select(x => x.Id).Contains(x.Id))
                .ToListAsync();

            lodedOrders.ForEach(async order => await SendNotification(admins, UserTypes.Admin, OrderStatus.Sended, order));

            var result = new ResponseAddOrderDto
            {
                Shops = lodedOrders.Select(x => new ShopCostDto
                {
                    ShopName = x.Shop()?.Name,
                    Cost = x.Cost()
                }).ToList()
            };
            result.SubTotal = result.Shops.Sum(x => x.Cost);
            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<List<CustomerOrderDto>>> GetCustomerOrders()
        {
            var orders = await _GetCustomerOrders();
            if (orders == null)
                _Operation.SetContent<List<CustomerOrderDto>>(OperationResultTypes.NotExist, "");

            return _Operation.SetSuccess(orders);
        }

        public async Task<OperationResult<CustomerOrderDto>> GetCustomerOrderById(Guid orderId)
        {
            var result = await _GetCustomerOrderById(orderId);
            if (result == null)
                return _Operation.SetContent<CustomerOrderDto>(OperationResultTypes.NotExist, "");

            return _Operation.SetSuccess(result);
        }

        private async Task<CustomerOrderDto> _GetCustomerOrderById(Guid orderId)
        {
            var order = await Context.Orders
                    .Where(order => order.Id == orderId)
                    .Include("OrderDetails.Product.Tag.Shop.Address")
                    .Include("OrderDetails.Product.PriceLogs")
                    .Include(x => x.OrderStatusLogs)
                    .Include(x => x.Address)
                    .SingleOrDefaultAsync();

            if (order == null)
                return null;

            var result = new CustomerOrderDto
            {
                Id = order.Id,
                SerialNumber = order.SerialNumber,
                DateCreated = order.DateCreated,
                ShopName = order.OrderDetails.Select(x => x.Product.Tag.Shop.Name).FirstOrDefault(),
                Status = order.Status().MapCustomer(),
                SubTotal = order.Cost(),
                DeliveryCost = order.DeliveryCost ?? 0,
                DriverNote = order.DriverNote,
                AddressTitle = order.Address.Title,
                CustomerId = order.Address.CustomerId.Value,
                Distance = Math.Round(new Point(order.Address.Lat, order.Address.Long).CalculateDistance(new Point(order.Shop().Address.Lat, order.Shop().Address.Long)) / 1000, 2),
                Time = order.ExpectedTime,
                TimeAmount = order.GetTime().Item1,
                TimeType = order.GetTime().Item2
            };

            return result;
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
                        Status = x.Status().MapCustomer(),
                        TimeAmount = x.GetTime().Item1,
                        TimeType = x.GetTime().Item2
                    }).ToListAsync();

            orders = orders.Where(x => x.Status <= CustomerOrderStatus.Completed).ToList();
            return orders;
        }

        private async Task<bool> _UpdateOrdersListCustomer(Guid id)
        {
            var result = await _GetCustomerOrders(id);
            if (result == null)
                return false;
            await orderHubContext.Clients.User(id.ToString()).UpdateCustomerOrders(result);
            return true;
        }

        private async Task<bool> _UpdateOrderCustomer(Guid orderId)
        {
            var result = await _GetCustomerOrderById(orderId);
            if (result == null)
                return false;
            await orderHubContext.Clients.User(result.CustomerId.ToString()).UpdateCustomerOrder(result);
            return true;
        }

        private static string GenerateSerialNumber(OrderTypes type)
            => (type == OrderTypes.Instant ? "A" : "B") + Helpers.GetNumberToken(5);

        private static bool ValidAction(OrderSet order, AppUser currentUser, OrderStatus newStatus)
            => CanCustomerCancel(order, currentUser, newStatus) || CanAdminAction(order, currentUser, newStatus)
            || CanDriverAction(order, currentUser, newStatus);

        #endregion

        #region Driver

        private async Task<bool> _DriverAvilable(Guid? driverId)
        {
            var id = driverId.HasValue ? driverId : Context.CurrentUserId;
            var driver = await Context.Drivers().Where(x => x.Id == id).SingleOrDefaultAsync();
            if (driver.Avilable())
            {
                return true;
            }
            return false;
        }

        private async Task _SendToDrivers(Guid orderId, List<Guid> driverIds)
        {
            var oldDriverOrders = await Context.OrderDrivers.Where(x => x.OrderId == orderId).ToDictionaryAsync(x => x.DriverId);

            var driverOrders = driverIds.Where(x => !oldDriverOrders.ContainsKey(x)).Select(d => new OrderDriver
            {
                DriverId = d,
                OrderId = orderId,
                OrderDriverType = OrderDriverType.Nothing
            });

            Context.OrderDrivers.AddRange(driverOrders);

            await Context.SaveChangesAsync();
        }

        private async Task<List<Guid>> GetAvilableDriverIds(Guid id, string latitude, string longitude, decimal total)
        {
            // Nearby Drivers (10)
            // Avilable
            // Online
            // Fixed Amount

            var drivers = await Context.Drivers()
                .Include(x => x.DriverOrders).ThenInclude(x => x.OrderStatusLogs)
                .Include(x => x.Payments)
                .Include(x => x.Address)
                .ToListAsync();

            var driverIds = drivers
                .Where(x => x.Avilable() && x.IsNotRefused(id) && x.FixedAmount() >= total)
                .OrderByDescending(driver => new Point(latitude, longitude)
                            .CalculateDistance(new Point(driver.Address.Lat, driver.Address.Long)))
                .Take(10).Select(x => x.Id).ToList();

            return driverIds;
        }

        public async Task<OperationResult<List<DriverOrderDto>>> GetAvilableOrders()
        {
            var result = await _GetDriverOrders();
            if (result == null)
                return _Operation.SetContent<List<DriverOrderDto>>(OperationResultTypes.NotExist, "");

            return _Operation.SetSuccess(result);
        }

        public async Task<OperationResult<DriverOrderDetailsDto>> GetCurrentOrder(Guid? id)
        {
            var orders = await Context.Orders.Include(x => x.OrderStatusLogs)
                .Include(x => x.Address).ThenInclude(x => x.Customer)
                .Include("OrderDetails.Product.Tag.Shop.Address")
                .Include("OrderDetails.Product.PriceLogs")
                .ToListAsync();

            var order = orders.Where(x => id.HasValue ? x.Id == id : (x.DriverId == Context.CurrentUserId &&
                    x.Status() == OrderStatus.Assigned || x.Status() == OrderStatus.Collected))
                .Select(x => new DriverOrderDetailsDto
                {
                    Id = x.Id,
                    SerialNumber = x.SerialNumber,
                    DateCreated = x.DateCreated,
                    Status = x.DriverStatus(),
                    CustomerImagePath = x.Address.Customer.IdentifierImagePath,
                    CustomerName = x.Address.Customer.FullName,
                    CustomerPhone = x.Address.Customer.PhoneNumber,
                    ShopName = x.Shop().Name,
                    ShopLat = x.Shop().Address.Lat,
                    ShopLng = x.Shop().Address.Long,
                    CustomerAddress = x.Address.Text,
                    DeliveryCost = x.DeliveryCost ?? 0,
                    SubTotal = x.Cost(),
                    Time = x.ExpectedTime,
                    TimeAmount = x.GetTime().Item1,
                    TimeType = x.GetTime().Item2
                }).FirstOrDefault();

            return _Operation.SetSuccess(order);
        }
        
        private async Task<List<DriverOrderDto>> _GetDriverOrders(Guid? id = null)
        {
            id = id.HasValue ? id.Value : Context.CurrentUserId;

            var drivers = await Context.Drivers().Include(x => x.DriverOrders).ThenInclude(x => x.OrderStatusLogs).Include(x => x.Address).ToListAsync();

            drivers = drivers.Where(x => x.Avilable()).ToList();

            var driver = drivers.Where(x => x.Id == id).FirstOrDefault();

            if (driver == null)
                return null;

            if (!driver.Avilable())
                return new List<DriverOrderDto>();

            var orders = await Context.Orders.Include(x => x.OrderStatusLogs)
                .Include("OrderDetails.Product.Tag.Shop.Address")
                .Include(x => x.Address).ThenInclude(x => x.Customer)
                .Include(x => x.OrderDrivers)
                .ToListAsync();

            var result = orders.Where(order => order.Status() == OrderStatus.Accepted && order.IsNotRefused(id.Value)
                    && drivers.OrderByDescending(driver
                        => new Point(order.Shop().Address.Lat, order.Shop().Address.Long)
                            .CalculateDistance(new Point(driver.Address.Lat, driver.Address.Long)))
                                .Take(10).Select(x => x.Id).Contains(driver.Id))
                .Select(x => new DriverOrderDto
                {
                    Id = x.Id,
                    SerialNumber = x.SerialNumber,
                    DateCreated = x.DateCreated,
                    Status = x.DriverStatus(),
                    CustomerName = x.Address.Customer.FullName,
                    CustomerImagePath = x.Address.Customer.IdentifierImagePath,
                    CustomerPhone = x.Address.Customer.PhoneNumber,
                    TimeAmount = x.GetTime().Item1,
                    TimeType = x.GetTime().Item2
                }).ToList();

            return result;
        }

        private static bool CanDriverAction(OrderSet order, AppUser currentUser, OrderStatus newStatus)
            => currentUser.UserType == UserTypes.Driver
                && (
                    (order.Status() == OrderStatus.Accepted && newStatus == OrderStatus.Assigned)
                 || (order.Status() == OrderStatus.Assigned && newStatus == OrderStatus.Collected)
                 || (order.Status() == OrderStatus.Collected && newStatus == OrderStatus.Completed)
                );

        public async Task<OperationResult<bool>> RefuseDriverOrder(Guid orderId)
        {
            var orderDriver = await Context.OrderDrivers.Where(x => x.OrderId == orderId && x.DriverId == Context.CurrentUserId).FirstOrDefaultAsync();

            if (orderDriver == null)
            {
                orderDriver = new OrderDriver();

                orderDriver.OrderId = orderId;
                orderDriver.DriverId = Context.CurrentUserId.Value;
                orderDriver.OrderDriverType = OrderDriverType.Refused;

                Context.OrderDrivers.Add(orderDriver);
            }
            else
            {
                orderDriver.OrderDriverType = OrderDriverType.Refused;
            }

            await Context.SaveChangesAsync();

            return _Operation.SetSuccess(true);
        }

        private async Task _AcceptDriverOrder(Guid orderId, Guid driverId)
        {
            var orderDriver = await Context.OrderDrivers.Where(x => x.OrderId == orderId && x.DriverId == driverId).FirstOrDefaultAsync();
            if (orderDriver == null)
            {
                orderDriver = new OrderDriver();
                orderDriver.DriverId = driverId;
                orderDriver.OrderId = orderId;
                orderDriver.OrderDriverType = OrderDriverType.Accepted;

                Context.OrderDrivers.Add(orderDriver);
            }
            else
            {
                orderDriver.OrderDriverType = OrderDriverType.Accepted;
            }
        }

        private async Task<bool> _UpdateOrdersListDriver(List<Guid> driverIds)
        {
            foreach (var driverId in driverIds)
            {
                var orders = await _GetDriverOrders(driverId);
                await orderHubContext.Clients.Users(driverIds.Select(x => x.ToString())).UpdateDriverOrders(orders);
            }
            return true;
        }

        private static bool CanCustomerCancel(OrderSet order, AppUser currentUser, OrderStatus newStatus)
           => currentUser.UserType == UserTypes.Customer && order.Status() == OrderStatus.Sended && newStatus == OrderStatus.Canceled;


        #endregion

        #region Shop

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
                    && (string.IsNullOrEmpty(search) || x.SerialNumber.Contains(search) || x.Cost().ToString().Contains(search)))
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
                        Price = x.Product.Price()
                    }).ToList(),
                    TimeAmount = x.GetTime().Item1,
                    TimeType = x.GetTime().Item2
                }).ToListAsync();

            return orders;
        }

        private async Task<bool> _UpdateOrdersListShop(Guid id, bool? isReady = false)
        {
            var result = await _GetShopOrders(isReady);
            await orderHubContext.Clients.User(id.ToString()).UpdateShopOrders(result);
            return true;
        }

        #endregion

        #region Dashboard

        private async Task<List<DashboardOrderDto>> _GetOrdersBoard()
        {
            var orders = await Context.Orders
                .Include(x => x.OrderStatusLogs)
                .Include(x => x.Address).ThenInclude(x => x.Customer)
                .Include(x => x.Driver)
                .ToListAsync();
            var result = orders.Select(x => new DashboardOrderDto
            {
                Id = x.Id,
                DateCreated = x.DateCreated,
                SerialNumber = x.SerialNumber,
                Status = x.CompanyStatus(),
                ImagePath = x.Status() == OrderStatus.Sended ? x.Address.Customer.IdentifierImagePath
                         : (x.Status() == OrderStatus.Accepted || x.Status() == OrderStatus.Refused || x.Status() == OrderStatus.Canceled ? null : x.Driver.IdentifierImagePath),
                PhoneNumber = x.Status() == OrderStatus.Sended ? x.Address.Customer.PhoneNumber
                         : (x.Status() == OrderStatus.Accepted || x.Status() == OrderStatus.Refused || x.Status() == OrderStatus.Canceled ? "" : x.Driver.PhoneNumber),
                FullName = x.Status() == OrderStatus.Sended ? x.Address.Customer.FullName
                         : (x.Status() == OrderStatus.Accepted ? "Unassigned" : (x.Status() == OrderStatus.Refused || x.Status() == OrderStatus.Canceled ? null : x.Driver.FullName)),
                TimeAmount = x.GetTime().Item1,
                TimeType = x.GetTime().Item2
            }).OrderBy(x => x.DateCreated).ToList();
            
            return result;
        }

        public async Task<OperationResult<List<DashboardOrderDto>>> GetOrdersBoard()
        {
            var orders = await _GetOrdersBoard();

            return _Operation.SetSuccess(orders);
        }

        public async Task<OperationResult<OrderDashboardDetails>> GetOrderDashboardDetails(Guid orderId)
        {
            var order = await Context.Orders
                .Where(x => x.Id == orderId)
                .Include(x => x.OrderStatusLogs)
                .Include(x => x.Address).ThenInclude(x => x.Customer)
                .Include("OrderDetails.Product.Tag.Shop.Address")
                .Include("OrderDetails.Product.Tag.Shop.Documents")
                .Include(x => x.Driver)
                .SingleOrDefaultAsync();
            if (order == null)
                return _Operation.SetContent<OrderDashboardDetails>(OperationResultTypes.NotExist, "");

            OrderDashboardDetails orderDashboardDetails = new OrderDashboardDetails();
            orderDashboardDetails.Id = order.Id;
            orderDashboardDetails.SerialNumber = order.SerialNumber;
            orderDashboardDetails.DateCreated = order.DateCreated;
            orderDashboardDetails.Status = OrderStatusHelper.MapCompany(order.Status());

            if (order.Status() == OrderStatus.Completed)
            {
                orderDashboardDetails.CompletedOn = order.OrderStatusLogs.OrderByDescending(x => x.DateCreated).Select(x => x.DateCreated).FirstOrDefault();
            }
            orderDashboardDetails.Details = (await GetOrderDetails(orderId)).Result;
            orderDashboardDetails.Customer = new UserInfoDto()
            {
                Id = order.Address.CustomerId.Value,
                ImagePath = order.Address.Customer.IdentifierImagePath,
                Address = order.Address.Text + " - " + order.Address.Building,
                Lat = order.Address.Lat,
                Lng = order.Address.Long,
                Name = order.Address.Customer.FullName,
                Note = order.DriverNote,
                PhoneNumber = order.Address.Customer.PhoneNumber
            };

            var shop = order.OrderDetails.Select(x => x.Product.Tag.Shop).FirstOrDefault();
            if (shop != null)
            {
                orderDashboardDetails.Shop = new UserInfoDto()
                {
                    Id = shop.Id,
                    ImagePath = shop.Documents.Select(x => x.Path).FirstOrDefault(),
                    Address = shop.Address?.Text,
                    Lat = shop.Address?.Lat,
                    Lng = shop.Address?.Long,
                    Name = shop.Name,
                    Note = order.ShopNote,
                    PhoneNumber = shop.PhoneNumber
                };
            }

            if (order.DriverId != null)
            {
                orderDashboardDetails.Driver = new UserInfoDto()
                {
                    Id = order.Driver.Id,
                    ImagePath = order.Driver.IdentifierImagePath,
                    Name = order.Driver.FullName,
                    Note = order.DriverNote,
                    PhoneNumber = order.Driver.PhoneNumber
                };
            }

            if (order.Status() == OrderStatus.Sended)
            {
                orderDashboardDetails.ExpectedCost = (await GetExpectedCost(order.AddressId)).Result;
            }
            else
            {
                orderDashboardDetails.ExpectedCost = new ExpectedCostDto
                {
                    Cost = order.ExpectedCost,
                    Time = order.ExpectedTime
                };
            }

            return _Operation.SetSuccess(orderDashboardDetails);

        }

        private async Task<bool> _UpdateOrdersListDashboard()
        {
            ///ToDo
            var admins = accountRepository.GetUserIds(UserTypes.Admin);
            var orders = await _GetOrdersBoard();
            await orderHubContext.Clients.Users(admins.Select(x => x.ToString())).UpdateAdminOrders(orders);
            return true;
        }

        private static bool CanAdminAction(OrderSet order, AppUser currentUser, OrderStatus newStatus)
            => currentUser.UserType == UserTypes.Admin
                && (
                    ((order.Status() == OrderStatus.Sended) && (newStatus == OrderStatus.Accepted || newStatus == OrderStatus.Refused))
                  || (order.Status() == OrderStatus.Accepted && newStatus == OrderStatus.Assigned)
                );

        #endregion

        #region Test

        public async Task<OperationResult<object>> NextStep(Guid? orderId, Guid? shopId, Guid? customerId, Guid? driverId)
        {
            var customer = await Context.Customers()
                .Include(x => x.Addresses).Where(x => x.Addresses.Any()
             && (customerId.HasValue && x.Id == customerId.Value) || true).FirstOrDefaultAsync();
            var shop = await Context.Shops().Include(x => x.Tags).ThenInclude(x => x.Products).Where(x => (shopId.HasValue && x.Id == shopId.Value) || x.Tags.Select(x => x.Products.Any()).Any()).FirstOrDefaultAsync();
            var driver = await Context.Drivers().Where(x => (driverId.HasValue && x.Id == driverId.Value) || true).FirstOrDefaultAsync();
            var admin = await Context.Admins().FirstOrDefaultAsync();

            var currentUser1 = Context.Users.Where(x => x.Id == Context.CurrentUserId).Include(x => x.Tags).ThenInclude(x => x.Products).Include(x => x.Addresses).FirstOrDefault();
            if (currentUser1 != null)
            {
                switch (currentUser1.UserType)
                {
                    case UserTypes.Admin:
                        admin = currentUser1;
                        break;
                    case UserTypes.Shop:
                        shop = currentUser1;
                        break;
                    case UserTypes.Customer:
                        customer = currentUser1;
                        break;
                    case UserTypes.Driver:
                        driver = currentUser1;
                        break;
                    default:
                        break;
                }
            }

            if (customer is null || shop is null || driver == null || admin == null)
                return _Operation.SetFailed<object>("");

            if (orderId == null)
            {
                await AddOrder(new SetOrderDto
                {
                    AddressId = customer.Addresses.FirstOrDefault().Id,
                    DriverNote = "DriverNote",
                    Cart = new List<ResponseCardDto>()
                    {
                        new ResponseCardDto()
                        {
                            Id = shop.Id,
                            Note = "ShopNote",
                            Products = new List<ProductCardDto>()
                            {
                                new ProductCardDto()
                                {
                                    Id = Context.Products.Where(x => x.Tag.ShopId == shop.Id).Select(x => x.Id).FirstOrDefault(),
                                    Count = 2,
                                }
                            }

                        }
                    }
                }, customer.Id);
                var order = await Context.Orders.Include(x => x.Address).Include("OrderDetails.Product.Tag").OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();
                return _Operation.SetSuccess<object>(new
                {
                    orderId = order.Id,
                    shopId = order.OrderDetails.Select(x => x.Product.Tag.ShopId).FirstOrDefault(),
                    customerId = order.Address.CustomerId,
                    DriverId = order.DriverId,
                    AdminId = admin.Id
                });
            }
            else
            {
                var currentStatus = await Context.OrderStatusLogs.Where(x => x.OrderId == orderId).OrderByDescending(x => x.DateCreated).Select(x => x.Status).FirstOrDefaultAsync();
                AppUser currentUser = null;
                switch (currentStatus)
                {
                    case OrderStatus.Sended:
                        currentStatus = OrderStatus.Accepted;
                        currentUser = admin;
                        break;
                    case OrderStatus.Accepted:
                        currentStatus = OrderStatus.Assigned;
                        currentUser = driver;
                        break;
                    case OrderStatus.Assigned:
                        currentStatus = OrderStatus.Collected;
                        currentUser = driver;
                        break;
                    case OrderStatus.Collected:
                        currentStatus = OrderStatus.Completed;
                        currentUser = driver;
                        break;
                    default:
                        return _Operation.SetSuccess<object>(false);
                }
                var order = await ChangeStatus(orderId.Value, currentStatus, currentUser);
                return _Operation.SetSuccess<object>(currentUser.Id);
            }
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

        public async Task<OperationResult<bool>> DeleteAll()
        {
            var orders = await Context.Orders.ToListAsync();
            Context.Orders.RemoveRange(orders);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        #endregion
    }
}
