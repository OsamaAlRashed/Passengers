using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Services.NotificationService
{
    public interface INotificationService
    {
        Task<bool> SendNotification(List<string> devicesTokens, string title, string body);
    }
}
