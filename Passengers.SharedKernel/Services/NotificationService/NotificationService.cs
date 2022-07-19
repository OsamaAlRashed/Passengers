using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        public async Task<bool> SendNotification(List<string> devicesTokens, string title, string body)
        {
            var message = new MulticastMessage()
            {
                Tokens = devicesTokens,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
            };

            var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            if (response.SuccessCount == devicesTokens.Count)
                return true;
            return false;
        }
    }
}
