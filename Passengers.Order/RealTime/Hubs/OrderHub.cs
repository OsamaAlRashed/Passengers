using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Passengers.DataTransferObject.OrderDtos;
using Passengers.Order.OrderService;
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
        public Task UpdateShopOrders(List<ShopOrderDto> orders);
        public Task UpdateDriverOrders(List<DriverOrderDto> orders);
        public Task UpdateAdminOrders(List<DashboardOrderDto> orders);

        //
        public Task Test(string text);
        public Task Test2(string text1, string text2);
        public Task ReceiveMessage(object text);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderHub : Hub<IOrderHub>
    {
        private readonly PassengersDbContext dbContext;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IOrderRepository orderRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public OrderHub(PassengersDbContext dbContext, IUserConnectionManager userConnectionManager,
            IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            _userConnectionManager = userConnectionManager;
            this.orderRepository = orderRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        
        public override Task OnConnectedAsync()
        {
            //var userId = dbContext.CurrentUserId;
            //if(userId == null)
            //{
            //    userId = GetUserId();
            //}
            //_userConnectionManager.KeepUserConnection(userId.Value, Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        
        public override Task OnDisconnectedAsync(Exception exception)
        {
            //var connectionId = Context.ConnectionId;
            //_userConnectionManager.RemoveUserConnection(connectionId);
            
            return base.OnDisconnectedAsync(exception);
        }

        public async Task BroadcastMessage(string message)
        {
            await Clients.All.ReceiveMessage(message);
        }

        private Guid? GetUserId()
        {
            HttpContext? httpContext = httpContextAccessor.HttpContext;
            var token = httpContextAccessor.HttpContext.Request.Query.Where(q => q.Key == "access_token").FirstOrDefault();
            var stream = token.Value;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            return tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value?.StringToGuid();
        }
    }
} 
