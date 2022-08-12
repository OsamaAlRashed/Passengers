using Passengers.DataTransferObject.OrderDtos;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.SettingService
{
    public interface ISettingRepository
    {
        Task<OperationResult<SettingDto>> GetSettings();
        Task<OperationResult<bool>> SetSettings(decimal price);
    }
}
