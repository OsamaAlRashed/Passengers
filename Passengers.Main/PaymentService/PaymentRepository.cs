using Passengers.DataTransferObject.PaymentDtos;
using Passengers.Main.PaymentService;
using Passengers.Models.Main;
using Passengers.Repository.Base;
using Passengers.SharedKernel.OperationResult;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<OperationResult<IEnumerable<PaymentDto>>> Get() => await base.GetAsync();
        public async Task<OperationResult<PaymentDto>> GetById(Guid id) => await base.GetByIdAsync(id);

    }
}
