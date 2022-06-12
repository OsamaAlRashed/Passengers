using Microsoft.AspNetCore.SignalR.Client;
using SharedClient;
using System;
using System.Threading.Tasks;

namespace CustomerClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var result = await Requst.Login("Customer", "0956057886", "1111");

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
