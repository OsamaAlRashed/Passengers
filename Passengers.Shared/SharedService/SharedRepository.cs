using Microsoft.EntityFrameworkCore;
using Passengers.Base;
using Passengers.DataTransferObject.BaseDtos;
using Passengers.DataTransferObject.SharedDtos;
using Passengers.Models.Base;
using Passengers.Repository.Base;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.SharedService
{
    public class SharedRepository : BaseRepository, ISharedRepository
    {
        public SharedRepository(PassengersDbContext context) : base(context) { }

        public async Task<bool> CheckIsExist<T>(Guid id) where T : BaseEntity
        {
            return await Context.Set<T>().Where(x => x.Id == id).AnyAsync();
        }

        public async Task<OperationResult<HomeDto>> GetHomeDetails()
        {
            HomeDto dto = new();
            dto.AdminCount = await Context.Admins().CountAsync();
            dto.DriverCount = await Context.Drivers().CountAsync();
            dto.ShopCount = await Context.Shops().CountAsync();
            dto.CustomerCount = await Context.Customers().CountAsync();

            dto.OrderStatisticDto = new OrderStatisticDto
            {
                OrderCount = await Context.Orders.CountAsync(),
                OrderMonthly = Enumerable.Range(1, 12)
                                           .GroupJoin(Context.Orders.Where(x => x.DateCreated.Year == DateTime.Now.Year).ToList(),
                                                        m => m,
                                                        x => x.DateCreated.Month, (m, order) => order.Count()).ToList()
            };

            dto.ImportStatisticDto = new ImportStatisticDto
            {
                TotalImport = (await Context.Payments.ToListAsync()).Where(x => !x.Type.IsExport()).Sum(x => x.Amount),
                ImportMonthly = Enumerable.Range(1, 12)
                                           .GroupJoin(Context.Payments.ToList().Where(x => x.Date.Year == DateTime.Now.Year && !x.Type.IsExport()),
                                                        m => m,
                                                        x => x.Date.Month, (m, import) => import.Sum(x => x.Amount)).ToList()
            };

            dto.ExportStatisticDto = new ExportStatisticDto
            {
                TotalExport = (await Context.Payments.ToListAsync()).Where(x => x.Type.IsExport()).Sum(x => x.Amount),
                ExportMonthly = Enumerable.Range(1, 12)
                                           .GroupJoin(Context.Payments.ToList().Where(x => x.Date.Year == DateTime.Now.Year && x.Type.IsExport()),
                                                        m => m,
                                                        x => x.Date.Month, (m, import) => import.Sum(x => x.Amount)).ToList()
            };

            dto.BestCustomers = (await Context.Customers()
                .Include("Addresses.Orders")
                .ToListAsync())
                .OrderByDescending(x => x.Addresses.Sum(x => x.Orders.Count()))
                .Take(2)
                .Select(x => new UserInfoDto
                {
                    Count = x.Addresses.Sum(x => x.Orders.Count()),
                    ImagePath = x.IdentifierImagePath,
                    Name = x.FullName
                }).ToList();

            dto.BestDrivers = (await Context.Drivers()
                .Include(x => x.DriverOrders)
                .Include(x => x.Documents)
                .ToListAsync())
                .OrderByDescending(x => x.DriverOrders.Count())
                .Take(2)
                .Select(x => new UserInfoDto
                {
                    Count = x.DriverOrders.Count(),
                    ImagePath = x.ImagePath(),
                    Name = x.FullName
                }).ToList();

            dto.BestShops = (await Context.Orders
                .Include("OrderDetails.Product.Tag.Shop.Documents").ToListAsync())
                .GroupBy(x => x.Shop())
                .OrderByDescending(x => x.Count())
                .Take(2)
                .Select(x => new UserInfoDto
                {
                    Name = x.Key.Name,
                    ImagePath = x.Key.ImagePath(),
                    Count = x.Count()
                }).ToList();
             
            return _Operation.SetSuccess(dto);
        }
    }
}
