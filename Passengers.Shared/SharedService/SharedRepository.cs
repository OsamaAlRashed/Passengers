using Microsoft.EntityFrameworkCore;
using Passengers.DataTransferObject.BaseDtos;
using Passengers.Models.Base;
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

namespace Passengers.Shared.SharedService
{
    public class SharedRepository : BaseRepository, ISharedRepository
    {
        public SharedRepository(PassengersDbContext context) : base(context) { }

        public async Task<bool> CheckIsExist<T>(Guid id) where T : BaseEntity
        {
            return await Context.Set<T>().Where(x => x.Id == id).AnyAsync();
        }
    }
}
