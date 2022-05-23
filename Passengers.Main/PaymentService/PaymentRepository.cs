using Microsoft.EntityFrameworkCore;
using Passengers.DataTransferObject.PaymentDtos;
using Passengers.Main.PaymentService;
using Passengers.Main.PaymentService.Store;
using Passengers.Models.Main;
using Passengers.Repository.Base;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SharedKernel.Services.Selector;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.SalaryLogService
{
    public class PaymentRepository : BaseRepository<Payment, PaymentDto>, IPaymentRepository
    {
        public PaymentRepository(PassengersDbContext context): base(context)
        {
        }

        public async Task<OperationResult<PaymentDto>> Add(PaymentDto salaryLogDto) => await base.AddAsync(salaryLogDto);
        public async Task<OperationResult<PaymentDto>> Update(PaymentDto salaryLogDto) => await base.UpdateAsync(salaryLogDto);
        public async Task<OperationResult<bool>> Delete(Guid id) => await base.DeleteAsync(id);
        public async Task<OperationResult<IEnumerable<PaymentDto>>> Get(int? year, int? month)
        {
            Expression<Func<Payment, PaymentDto>> selector = Selector.GetSelector<Payment, PaymentDto>();
            IEnumerable<PaymentDto> entities = await Context.Set<Payment>()
                .Where(PaymentStore.Filter.WhereDateFilter(year, month))
                .Select(selector).OrderByDescending(x => x.Date)
                .ToListAsync();

            return _Operation.SetSuccess(entities);
        }
        public async Task<OperationResult<PaymentDto>> GetById(Guid id) => await base.GetByIdAsync(id);
        public async Task<OperationResult<ImportPaymentDto>> Import(ImportPaymentDto dto)
        {
            var entity = PaymentStore.Query.ImportDtoToPayment(dto);
            Context.Payments.Add(entity);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(PaymentStore.Query.PaymentToImportDto.Compile()(entity));
        }
        public async Task<OperationResult<ExportPaymentDto>> Export(ExportPaymentDto dto)
        {
            var entity = PaymentStore.Query.ExportDtoToPayment(dto);
            Context.Payments.Add(entity);
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(PaymentStore.Query.PaymentToExportDto.Compile()(entity));
        }

        public async Task<OperationResult<object>> GetNameAndSalary(Guid userId)
        {
            var data = await Context.Users.Where(x => x.Id == userId).Select(x => new { Name = x.FullName, Amount = x.Salary }).SingleOrDefaultAsync();
            if (data is null)
                return _Operation.SetContent(OperationResultTypes.NotExist, "");
            return _Operation.SetSuccess<object>(data);
        }

        public Task<decimal> GetTotalFixedAmont(Guid? driverId)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetExportFixedAmont(Guid? driverId)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetImportFixedAmont(Guid? driverId)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetDeliveriesAmount(Guid? driverId)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotal(PaymentType? type, Guid? userId)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<List<SalaryPaymentDto>>> GetSalaries(int? year, int? month)
        {
            var data = await Context.Payments
                .Where(x => x.Type == PaymentType.Salary)
                .Where(PaymentStore.Filter.WhereDateFilter(year, month))
                .Select(PaymentStore.Query.PaymentToSalaryDto)
                .ToListAsync();

            return _Operation.SetSuccess(data);
        }

        public async Task<OperationResult<List<ImportPaymentDto>>> GetImports(int? year, int? month)
        {
            var data = await Context.Payments
                .Where(x => x.Type == PaymentType.Delivery || x.Type == PaymentType.FixedImport || x.Type == PaymentType.OtherImport)
                .Where(PaymentStore.Filter.WhereDateFilter(year, month))
                .Select(PaymentStore.Query.PaymentToImportDto)
                .ToListAsync();

            return _Operation.SetSuccess(data);
        }

        public async Task<OperationResult<List<ExportPaymentDto>>> GetExports(int? year, int? month)
        {
            var data = await Context.Payments
                .Where(x => x.Type == PaymentType.FixedExport || x.Type == PaymentType.OtherExport)
                .Where(PaymentStore.Filter.WhereDateFilter(year, month))
                .Select(PaymentStore.Query.PaymentToExportDto)
                .ToListAsync();

            return _Operation.SetSuccess(data);
        }
    }
}
