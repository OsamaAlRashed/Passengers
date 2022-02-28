using Passengers.Models.Base;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.SharedService
{
    public interface ISharedRepository
    {
        Task<bool> CheckIsExist<T>(Guid id) where T : BaseEntity;
    }
}
