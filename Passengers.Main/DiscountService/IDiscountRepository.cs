using Passengers.DataTransferObject.DiscountDtos;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.DiscountService
{
    public interface IDiscountRepository
    {
        Task<OperationResult<DiscountDto>> GetById(Guid id);
        Task<OperationResult<DiscountDto>> GetActiveByProductId(Guid productId);
        Task<OperationResult<List<DiscountDto>>> GetAllByProductId(Guid productId);
        Task<OperationResult<List<DiscountDto>>> Get();
        Task<OperationResult<DiscountDto>> Add(DiscountDto dto);
        Task<OperationResult<bool>> EditEndDate(Guid productId, DateTime? endDate);
        Task<OperationResult<bool>> DeleteActiveByProductId(Guid productId);
        Task<OperationResult<bool>> DeleteAllByProductId(Guid productId);
        Task<OperationResult<bool>> Delete(Guid id);
    }
}
