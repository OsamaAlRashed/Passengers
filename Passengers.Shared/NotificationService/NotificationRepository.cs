using Passengers.Repository.Base;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Passengers.Shared.NotificationService
{
    public class NotificationRepository : BaseRepository, INotificationRepository
    {
        private readonly IHttpClientFactory httpClient;

        public NotificationRepository(PassengersDbContext context, IHttpClientFactory httpClient): base(context)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> FireAsync(string applicationId, string deviceId, string title, string body, object data)
        {

            var _fcmHttp = httpClient.CreateClient("fcm");
            var request = new
            {
                to = deviceId,
                notification = new
                {
                    title = title,
                    body = body,
                },
                data = data,
            };
            var json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            _fcmHttp.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={applicationId}");
            _fcmHttp.DefaultRequestHeaders.TryAddWithoutValidation("Sender", $"id={applicationId}");
            return await _fcmHttp.PostAsync("fcm/send", httpContent);
        }

    }
}
