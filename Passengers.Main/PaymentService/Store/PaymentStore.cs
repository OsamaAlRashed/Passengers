using Passengers.DataTransferObject.PaymentDtos;
using Passengers.Models.Base;
using Passengers.Models.Main;
using Passengers.SharedKernel.ExtensionMethods;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.PaymentService.Store
{
    public static class PaymentStore
    {

        public static class Filter
        {
            public static Expression<Func<Payment, bool>> WhereDateFilter(int? year, int? month) => x =>
                (!year.HasValue || x.Date.Year == year)
                && (!month.HasValue || x.Date.Month == month)
                && ((!year.HasValue && month.HasValue) ? x.Date.Year == DateTime.Now.Year : true);
        }
        public static class Query
        {
            public static Func<ImportPaymentDto, Payment> ImportDtoToPayment => c => new Payment
            {
                Id = c.Id,
                Amount = c.Amount,
                Date = c.Date,
                Note = c.Note,
                Type = SharedKernel.Enums.PaymentTypesHelper.Map(c.Type),
                UserId = c.UserId == Guid.Empty ? null : c.UserId,
            };

            public static Func<ExportPaymentDto, Payment> ExportDtoToPayment => c => new Payment
            {
                Id = c.Id,
                Amount = c.Amount,
                Date = c.Date,
                Note = c.Note,
                Type = SharedKernel.Enums.PaymentTypesHelper.Map(c.Type),
                UserId = c.UserId == Guid.Empty ? null : c.UserId,
            };

            public static Expression<Func<Payment, ExportPaymentDto>> PaymentToExportDto => c => new ExportPaymentDto
            {
                Id = c.Id,
                Amount = c.Amount,
                Date = c.Date,
                Note = c.Note,
                Type = SharedKernel.Enums.PaymentTypesHelper.MapExport(c.Type),
                UserId = c.UserId,
                DateCreated = c.DateCreated,
                DateDeleted = c.DateDeleted,
                DateUpdated = c.DateUpdated,
                ActionBy = PassengersDbContext.LastActionBy(c)
            };

            public static Expression<Func<Payment, ImportPaymentDto>> PaymentToImportDto => c => new ImportPaymentDto
            {
                Id = c.Id,
                Amount = c.Amount,
                Date = c.Date,
                Note = c.Note,
                Type = SharedKernel.Enums.PaymentTypesHelper.MapImport(c.Type),
                UserId = c.UserId,
                DateCreated = c.DateCreated,
                DateDeleted = c.DateDeleted,
                DateUpdated = c.DateUpdated,
                ActionBy = PassengersDbContext.LastActionBy(c),
            };

            public static Expression<Func<Payment, SalaryPaymentDto>> PaymentToSalaryDto => c => new SalaryPaymentDto
            {
                Id = c.Id,
                Amount = c.Amount,
                Date = c.Date,
                Note = c.Note,
                UserId = c.UserId.GetValueOrDefault(),
                DateCreated = c.DateCreated,
                DateDeleted = c.DateDeleted,
                DateUpdated = c.DateUpdated,
                ActionBy = PassengersDbContext.LastActionBy(c)
            };

        }
    }
}
        
