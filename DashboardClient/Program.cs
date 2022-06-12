using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace DashboardClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var result = await Requst.Login("Account", "AdminEsbQ8v", "1111");

            var hubConnection = new HubConnectionBuilder()
                         .WithUrl("https://localhost:44327/orderHub", options =>
                         {
                             options.AccessTokenProvider = () => Task.FromResult(result.AccessToken);
                         }).Build();

            hubConnection.On<object>("NewOrder",
                order =>
                {
                    Console.WriteLine("NewOrder");
                });

            hubConnection.On<object>("UpdateOrder",
                order =>
                {
                    Console.WriteLine("UpdateOrder");
                });

            hubConnection.On<object>("ChangeStatus",
                order =>
                {
                    Console.WriteLine("ChangeStatus");
                });

            hubConnection.On<Guid>("RemoveOrder",
                order =>
                {
                    Console.WriteLine("RemoveOrder");
                });

            try
            {
                await hubConnection.StartAsync();
                Console.Read();
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
