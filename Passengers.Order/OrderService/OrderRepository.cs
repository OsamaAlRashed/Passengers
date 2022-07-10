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
        #region Ctor & Props

        private readonly IHubContext<OrderHub, IOrderHub> orderHubContext;
        private readonly IUserConnectionManager userConnectionManager;
        private readonly IAccountRepository accountRepository;

        public OrderRepository(PassengersDbContext context, IHubContext<OrderHub, IOrderHub> orderHubContext, IUserConnectionManager userConnectionManager, IAccountRepository accountRepository) : base(context)
        {
            this.orderHubContext = orderHubContext;
            this.userConnectionManager = userConnectionManager;
            this.accountRepository = accountRepository;
        }

        #endregion

        #region Public Method

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


            if(currentUserId == null)
                currentUserId = Context.CurrentUserId;
            await _UpdateOrdersListCustomer(currentUserId.Value);
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

        public async Task<OperationResult<bool>> ChangeStatus(Guid orderId, OrderStatus newStatus, AppUser? mockCurrentUser = null, decimal deliveryCost = 0, int expectedTime = 0, string reasonRefuse = "")
        {
            var currentUser = await Context.Users.FindAsync(Context.CurrentUserId);
            if(mockCurrentUser != null)
            {
                currentUser = mockCurrentUser;
            }
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
                    Note = newStatus == OrderStatus.Refused ? reasonRefuse : null,
                    OrderId = order.Id
                });

                if(newStatus == OrderStatus.Assigned)
                {
                    order.DriverId = currentUser.Id;
                }

                if(newStatus == OrderStatus.Accepted)
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
                    ImagePath = x.Product.ImagePath,
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

        public async Task<OperationResult<List<CustomerOrderDto>>> GetCustomerOrders()
        {
            var orders = await _GetCustomerOrders();
            if(orders == null)
                _Operation.SetContent<CustomerOrderDto>(OperationResultTypes.NotExist, "");

            return _Operation.SetSuccess(orders);
        }

        public async Task<OperationResult<CustomerOrderDto>> GetCustomerOrderById(Guid orderId)
        {
            var result = await _GetCustomerOrderById(orderId);
            if (result == null)
                return _Operation.SetContent<CustomerOrderDto>(OperationResultTypes.NotExist, "");

            return _Operation.SetSuccess(result);
        }


        public async Task<OperationResult<object>> NextStep(Guid? orderId, Guid? shopId, Guid? customerId, Guid? driverId)
        {
            var customer = await Context.Customers().Include(x => x.Addresses).Where(x => (customerId.HasValue && x.Id == customerId.Value) || true).FirstOrDefaultAsync();
            var shop = await Context.Shops().Include(x => x.Tags).ThenInclude(x => x.Products).Where(x => (shopId.HasValue && x.Id == shopId.Value) || x.Tags.Select(x => x.Products.Any()).Any()).FirstOrDefaultAsync();
            var driver = await Context.Drivers().Where(x => (driverId.HasValue && x.Id == driverId.Value) || true).FirstOrDefaultAsync();
            var admin = await Context.Admins().FirstOrDefaultAsync();
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

        public async Task<OperationResult<List<DashboardOrderDto>>> GetOrdersBoard()
        {
            var orders = await Context.Orders
                .Select(x => new DashboardOrderDto
                {
                    Id = x.Id,
                    DateCreated = x.DateCreated,
                    SerialNumber = x.SerialNumber,
                    Status = OrderStatusHelper.MapCompany(x.OrderStatusLogs.OrderBy(x => x.DateCreated).Select(x => x.Status).LastOrDefault()),
                    ImagePath = x.OrderStatusLogs.OrderBy(x => x.DateCreated).Select(x => x.Status).LastOrDefault() == OrderStatus.Sended ? x.Address.Customer.IdentifierImagePath
                        : (x.OrderStatusLogs.OrderBy(x => x.DateCreated).Select(x => x.Status).LastOrDefault() == OrderStatus.Accepted ? null : x.Driver.IdentifierImagePath),
                    PhoneNumber = x.OrderStatusLogs.OrderBy(x => x.DateCreated).Select(x => x.Status).LastOrDefault() == OrderStatus.Sended ? x.Address.Customer.PhoneNumber
                        : (x.Status == OrderStatus.Accepted ? "" : x.Driver.PhoneNumber),
                    FullName = x.OrderStatusLogs.OrderBy(x => x.DateCreated).Select(x => x.Status).LastOrDefault() == OrderStatus.Sended ? x.Address.Customer.FullName
                        : (x.Status == OrderStatus.Accepted ? "Unassigned" : x.Driver.FullName),
                    Time = DateTime.Now.Subtract(x.OrderStatusLogs.OrderBy(x => x.DateCreated).Select(x => x.DateCreated).LastOrDefault()).Minutes,
                }).ToListAsync();

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
            orderDashboardDetails.Status = OrderStatusHelper.MapCompany(order.Status);

            if (order.Status == OrderStatus.Completed)
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

            if (order.Status == OrderStatus.Sended)
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
        
        public async Task<List<Guid>> GetAvilableDriverIds(string latitude, string longitude, decimal total)
        {

            //Nearby Drivers
            //Avilable
            //Online
            //Fixed Amount

            var notAvilableDrivers = (await Context.Orders.Include(x => x.OrderStatusLogs).ToListAsync())
                .Where(x => x.Status >= OrderStatus.Assigned && x.Status <= OrderStatus.Accepted)
                .Select(x => x.DriverId).ToList();

            var drivers = (await Context.Drivers().Include(x => x.Payments).ToListAsync())
                .Where(x => x.DriverOnline.HasValue && x.DriverOnline.Value && !notAvilableDrivers.Contains(x.Id)
                    && x.Payments.Where(payment => payment.Type.IsFixed())
                                 .Sum(payment => payment.Amount * payment.Type.PaymentSign()) >= total)
                .OrderByDescending(x => CalculateDistance(new ValueTuple<string, string>(latitude, longitude), new ValueTuple<string, string>(x.Address.Lat, x.Address.Long)))
                .Take(5).ToList();

            return drivers.Select(x => x.Id).ToList();
        }

        #endregion

        #region Helpers

        private async Task<CustomerOrderDto> _GetCustomerOrderById(Guid orderId)
        {
            var customer = await Context.Customers().Include(x => x.Addresses)
                .Where(x => x.Id == Context.CurrentUserId).SingleOrDefaultAsync();
            if (customer == null)
                return null;

            var order = await Context.Orders
                    .Where(order => order.Id == orderId)
                    .Include("OrderDetails.Product.Tag.Shop")
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
                Status = OrderStatusHelper.MapCustomer(order.Status),
                SubTotal = order.OrderDetails.Select(x => new
                {
                    Price = x.Product.Price,
                    Quantity = x.Quantity
                }).Sum(x => x.Price * x.Quantity),
                DeliveryCost = order.DeliveryCost ?? 0,
                DriverNote = order.DriverNote,
                AddressTitle = order.Address.Title,
                CustomerId = order.Address.CustomerId.Value,
                ///TODO
                Distance = 100,
                Time = 10,
            };

            result.TotalCost = result.SubTotal + (result.DeliveryCost ?? 0);

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
                        Status = OrderStatusHelper.MapCustomer(x.Status),
                    }).ToListAsync();

            orders = orders.Where(x => x.Status <= CustomerOrderStatus.Completed).ToList();
            return orders;
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
                        Price = x.Product.PriceLogs
                            .Where(x => x.DateCreated <= DateTime.Now && (!x.EndDate.HasValue || DateTime.Now <= x.EndDate))
                            .Select(x => x.Price)
                            .FirstOrDefault()
                    }).ToList(),
                    //TotalPrice = x.OrderDetails.Sum(x => x.Quantity * x.Product.PriceLogs
                    //    .Where(x => x.DateCreated <= DateTime.Now && (!x.EndDate.HasValue || DateTime.Now <= x.EndDate))
                    //    .Select(x => x.Price)
                    //    .FirstOrDefault())
                }).ToListAsync();

            foreach (var order in orders)
            {
                order.TotalPrice = order.Products.Sum(x => x.Count * x.Price);
            }

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
                await _UpdateOrderCustomer(order.Id);
            }
            else if(newStatus == OrderStatus.Accepted)
            {
                await _UpdateOrdersListShop(shopId);
                await _UpdateOrdersListCustomer(customerId);
                await _UpdateOrderCustomer(order.Id);

                //Drivers
                var drivers = (await GetAvilableDriverIds(order.Address.Lat, order.Address.Long, order.DeliveryCost ?? 0)).Where(id => id != driverId.Value).ToList();
            }
            else if(newStatus == OrderStatus.Refused)
            {
                await _UpdateOrdersListCustomer(customerId);
                await _UpdateOrderCustomer(order.Id);
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
                await _UpdateOrderCustomer(order.Id);
            }
            else if (newStatus == OrderStatus.Completed)
            {
                await _UpdateOrdersListCustomer(customerId);
                await _UpdateOrderCustomer(order.Id);
            }
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
