using Microsoft.EntityFrameworkCore;
using Passengers.DataTransferObject.OrderDtos;
using Passengers.Repository.Base;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.SettingService
{
    public class SettingRepository : BaseRepository, ISettingRepository
    {
        public SettingRepository(PassengersDbContext context): base(context)
        {

        }
        public async Task<OperationResult<SettingDto>> GetSettings()
        {
            var setting = await Context.Settings.FirstOrDefaultAsync();
            
            return _Operation.SetSuccess(new SettingDto { KmPrice = setting.KMPrice });
        }

        public async Task<OperationResult<bool>> SetSettings(decimal price)
        {
            var setting = await Context.Settings.FirstOrDefaultAsync();
            setting.KMPrice = price;
            await Context.SaveChangesAsync();
            
            return _Operation.SetSuccess(true);
        }
    }
}
