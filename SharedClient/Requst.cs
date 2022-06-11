using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharedClient
{
    public class Requst
    {
        public static async Task<LoginResponse> Login(string controllerName, string userName, string password)
        {
            string baseUrl = "https://localhost:44327";
            HttpClient httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(new { userName = userName, password = password });
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{baseUrl}/api/{controllerName}/Login", data);
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponse>(result);
        }
    }

    public class Order
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; }
    }
}
