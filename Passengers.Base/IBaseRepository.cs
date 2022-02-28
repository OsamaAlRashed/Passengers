using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Base
{
    public interface IBaseRepository<T> where T : class
    {
        Task<OperationResult<IEnumerable<T>>> Get();
        Task<OperationResult<T>> GetById(Guid id);
        Task<OperationResult<T>> Add(T dto);
        Task<OperationResult<T>> Update(T dto);
        Task<OperationResult<bool>> Remove(Guid id);
    }
}
