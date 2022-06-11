using Microsoft.AspNetCore.SignalR.Client;
using SharedClient;
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

            hubConnection.On<string>("NewOrder",
                order =>
                {
                    Console.WriteLine(order);
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
