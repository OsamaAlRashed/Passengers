using Passengers.DataTransferObject.NotificationDtos;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.NotificationService
{
    public interface INotificationRepository
    {
        Task<OperationResult<List<NotificationDto>>> Get();
        Task<OperationResult<NotificationDto>> Add(NotificationDto dto);
    }
}
