using Passengers.DataTransferObject.DriverDtos;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.DriveService
{
    public interface IDriverRepository
    {
        Task<OperationResult<List<GetDriverDto>>> Get();
        Task<OperationResult<GetDriverDto>> GetById(Guid id);
        Task<OperationResult<GetDriverDto>> Add(SetDriverDto dto);
        Task<OperationResult<GetDriverDto>> Update(SetDriverDto dto);
        Task<OperationResult<bool>> Delete(Guid id);
    }
}
