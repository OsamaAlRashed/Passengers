using Passengers.DataTransferObject.PaymentDtos;
using Passengers.SharedKernel.Enums;
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
        Task<OperationResult<IEnumerable<PaymentDto>>> Get(int? year, int? month);
        Task<OperationResult<List<SalaryPaymentDto>>> GetSalaries(int? year, int? month);
        Task<OperationResult<List<ImportPaymentDto>>> GetImports(int? year, int? month);
        Task<OperationResult<List<ExportPaymentDto>>> GetExports(int? year, int? month);
        Task<OperationResult<PaymentDto>> Add(PaymentDto dto);
        Task<OperationResult<ImportPaymentDto>> Import(ImportPaymentDto dto);
        Task<OperationResult<ExportPaymentDto>> Export(ExportPaymentDto dto);
        Task<OperationResult<PaymentDto>> Update(PaymentDto salaryLogDto);
        Task<OperationResult<bool>> Delete(Guid id);
        Task<OperationResult<object>> GetNameAndSalary(Guid userId);

        Task<decimal> GetTotalFixedAmont(Guid? driverId);
        Task<decimal> GetExportFixedAmont(Guid? driverId);
        Task<decimal> GetImportFixedAmont(Guid? driverId);
        Task<decimal> GetDeliveriesAmount(Guid? driverId);
        Task<decimal> GetTotal(PaymentType? type, Guid? userId);
    }
}
