using Passengers.DataTransferObject.PaymentDtos;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.PaymentService
{
    public interface IPaymentRepository
    {
        Task<OperationResult<PaymentDto>> GetById(Guid id);
        Task<OperationResult<IEnumerable<PaymentDto>>> Get();
        Task<OperationResult<PaymentDto>> Add(PaymentDto salaryLogDto);
        Task<OperationResult<PaymentDto>> Update(PaymentDto salaryLogDto);
        Task<OperationResult<bool>> Delete(Guid id);
    }
}
