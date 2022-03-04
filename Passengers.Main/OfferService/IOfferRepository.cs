using Passengers.DataTransferObject.OfferDtos;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.OfferService
{
    public interface IOfferRepository
    {
        Task<OperationResult<List<GetOfferDto>>> Get(OfferTypes type, int pageSize, int pageNumber);
        Task<OperationResult<GetOfferDto>> GetById(Guid id);
        Task<OperationResult<GetOfferDto>> Add(SetOfferDto dto);
        Task<OperationResult<GetOfferDto>> Update(SetOfferDto dto);
        Task<OperationResult<bool>> Remove(Guid id);
        Task<OperationResult<bool>> Extension(Guid id, DateTime? endDate);
    }
}
