using Microsoft.EntityFrameworkCore;
using Passengers.DataTransferObject.DiscountDtos;
using Passengers.Main.DiscountService.Store;
using Passengers.Repository.Base;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.DiscountService
{
    public class DiscountRepository : BaseRepository, IDiscountRepository
    {
        public DiscountRepository(PassengersDbContext context):base(context)
        {

        }

        public async Task<OperationResult<DiscountDto>> Add(DiscountDto dto)
        {
            var currentDiscount = await Context.Discounts
                .Where(x => x.ProductId == dto.ProductId && x.StartDate <= DateTime.Now && DateTime.Now <= x.EndDate)
                .AnyAsync();
            if (currentDiscount)
                return _Operation.SetFailed<DiscountDto>("DiscountAlreadyExist");

            var entity = DiscountStore.Query.SetSelectDiscount.Compile()(dto);
            Context.Discounts.Add(entity);
            await Context.SaveChangesAsync();
            dto.Id = entity.Id;
            return _Operation.SetSuccess(dto);
        }

        public async Task<OperationResult<bool>> Delete(Guid id)
        {
            var entity = await Context.Discounts.Where(x => x.Id == id).SingleOrDefaultAsync();
            if (entity == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "DiscountNotFound");
            entity.DateDeleted = DateTime.Now;
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> DeleteActiveByProductId(Guid productId)
        {
            var discounts = await Context.Discounts
               .Where(x => x.ProductId == productId && x.StartDate <= DateTime.Now && DateTime.Now <= x.EndDate)
               .ToListAsync();

            List<Task> tasks = new();
            foreach (var discount in discounts)
            {
                tasks.Add(Delete(discount.Id));
            }
            await Task.WhenAll(tasks);
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> DeleteAllByProductId(Guid productId)
        {
            var discounts = await Context.Discounts
               .Where(x => x.ProductId == productId)
               .ToListAsync();

            List<Task> tasks = new();
            foreach (var discount in discounts)
            {
                tasks.Add(Delete(discount.Id));
            }
            await Task.WhenAll(tasks);
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<bool>> EditEndDate(Guid productId, DateTime? endDate)
        {
            var currentDiscount = await Context.Discounts
                .Where(x => x.ProductId == productId && x.StartDate <= DateTime.Now && DateTime.Now <= x.EndDate)
                .FirstOrDefaultAsync();
            if (currentDiscount == null)
                return _Operation.SetContent<bool>(OperationResultTypes.NotExist, "DiscountNotFound");
            currentDiscount.EndDate = endDate ?? DateTime.Now;
            await Context.SaveChangesAsync();
            return _Operation.SetSuccess(true);
        }

        public async Task<OperationResult<List<DiscountDto>>> Get()
        {
            var discounts = await Context.Discounts
               .Select(DiscountStore.Query.GetSelectDiscount)
               .ToListAsync();
            return _Operation.SetSuccess(discounts);
        }

        public async Task<OperationResult<DiscountDto>> GetActiveByProductId(Guid productId)
        {
            var discounts = await Context.Discounts
               .Where(x => x.ProductId == productId && x.StartDate <= DateTime.Now && DateTime.Now <= x.EndDate)
               .Select(DiscountStore.Query.GetSelectDiscount)
               .FirstOrDefaultAsync();
            return _Operation.SetSuccess(discounts);
        }

        public async Task<OperationResult<List<DiscountDto>>> GetAllByProductId(Guid productId)
        {
            var discounts = await Context.Discounts
               .Where(x => x.ProductId == productId)
               .Select(DiscountStore.Query.GetSelectDiscount)
               .ToListAsync();
            return _Operation.SetSuccess(discounts);
        }

        public async Task<OperationResult<DiscountDto>> GetById(Guid id)
        {
            var discounts = await Context.Discounts
               .Where(x => x.Id == id)
               .Select(DiscountStore.Query.GetSelectDiscount)
               .FirstOrDefaultAsync();
            return _Operation.SetSuccess(discounts);
        }
    }
}
