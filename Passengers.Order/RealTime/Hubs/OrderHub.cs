﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Passengers.Order.OrderService;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderSet = Passengers.Models.Order.Order;

namespace Passengers.Order.RealTime.Hubs
{
    public interface IOrderHub
    {
        public Task NewOrder(object order);
        public Task UpdateOrder(object order);
        public Task RemoveOrder(Guid id);
        public Task Test(string text);
        public Task Test2(string text1, string text2);
        public Task ReceiveMessage(object text);
    }

    public class OrderHub : Hub<IOrderHub>
    {
        private readonly PassengersDbContext dbContext;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IOrderRepository orderRepository;
        public OrderHub(PassengersDbContext dbContext, IUserConnectionManager userConnectionManager, IOrderRepository orderRepository)
        {
            this.dbContext = dbContext;
            _userConnectionManager = userConnectionManager;
            this.orderRepository = orderRepository;
        }
        
        public override Task OnConnectedAsync()
        {
            _userConnectionManager.KeepUserConnection(dbContext.CurrentUserId.Value, Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            _userConnectionManager.RemoveUserConnection(connectionId);
            
            return base.OnDisconnectedAsync(exception);
        }

        public async Task BroadcastMessage(string message)
        {
            await Clients.All.ReceiveMessage(message);
        }
    }
} 
