using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Passengers.DataTransferObject.OrderDtos;
using Passengers.Order.OrderService;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using OrderSet = Passengers.Models.Order.Order;

namespace Passengers.Order.RealTime.Hubs
{
    public interface IOrderHub
    {
        public Task UpdateCustomerOrders(List<CustomerOrderDto> orders);
        public Task UpdateCustomerOrder(CustomerOrderDto order);


        public Task UpdateShopOrders(List<ShopOrderDto> orders);

        public Task UpdateDriverOrders(List<DriverOrderDto> orders);

        public Task UpdateAdminOrders(List<DashboardOrderDto> orders);

        public Task UpdateLocation(string lat, string lng);

        public Task Test(string text);
        public Task Test2(string text1, string text2);
        public Task ReceiveMessage(object text);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderHub : Hub<IOrderHub>
    {
        private readonly PassengersDbContext context;

        public OrderHub(PassengersDbContext context)
        {
            this.context = context;
        }

        public async Task BroadcastMessage(string message)
        {
            await Clients.All.ReceiveMessage(message);
        }

        public async Task ChangeLocation(string lat, string lng)
        {
            var orders = await context.Orders
                .Include(x => x.OrderStatusLogs)
                .Include(x => x.Address)
                .Where(x => x.DriverId == context.CurrentUserId).ToListAsync();

            var currentCustomerId = orders.Where(x => x.Status() == OrderStatus.Assigned || x.Status() == OrderStatus.Collected).Select(x => x.Address.CustomerId).FirstOrDefault();

            if(currentCustomerId != null)
                await Clients.User(currentCustomerId.ToString()).UpdateLocation(lat, lng);
        }


        public async Task ChangeLocationTest(string lat, string lng)
        {
            await Clients.User("06fb27cb-9ae8-44c4-4921-08da7de6f854").UpdateLocation(lat, lng);
        }

        #region Connections
        //public override Task OnConnectedAsync()
        //{
        //    var userId = dbContext.CurrentUserId;
        //    if(userId == null)
        //    {
        //        userId = GetUserId();
        //    }
        //    _userConnectionManager.KeepUserConnection(userId.Value, Context.ConnectionId);
        //    return base.OnConnectedAsync();
        //}

        //public override Task OnDisconnectedAsync(Exception exception)
        //{
        //    var connectionId = Context.ConnectionId;
        //    _userConnectionManager.RemoveUserConnection(connectionId);

        //    return base.OnDisconnectedAsync(exception);
        //}
        #endregion

    }
} 
