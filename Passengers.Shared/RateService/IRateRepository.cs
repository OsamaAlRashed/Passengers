using Passengers.DataTransferObject.RateDtos;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.RateService
{
    public interface IRateRepository
    {
        Task<OperationResult<RateDto>> Get();
        Task<OperationResult<RateDto>> GetByEntityId(Guid entityId);
        Task<OperationResult<RateDto>> GetById(Guid id);
        Task<OperationResult<RateDto>> Add(RateDto rateDto);
        Task<OperationResult<RateDto>> Update(RateDto rateDto);
        Task<OperationResult<bool>> Remove(Guid id);

    }
}
