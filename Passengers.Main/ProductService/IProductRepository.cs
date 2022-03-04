using Passengers.DataTransferObject.ProductDtos;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.ProductService
{
    public interface IProductRepository
    {
        Task<OperationResult<GetProductDto>> Add(SetProductDto dto);
        Task<OperationResult<GetProductDto>> Update(SetProductDto dto);
        Task<OperationResult<bool>> Remove(Guid id);
        Task<OperationResult<bool>> ChangeAvilable(Guid id);
        Task<OperationResult<List<GetProductDto>>> Get();
        Task<OperationResult<object>> GetById(Guid id);
        Task<OperationResult<object>> GetFoodMenu(Guid tagId, int pageSize = 10, int pageNumber = 1);
        Task<OperationResult<bool>> ChangePrice(Guid id, decimal newPrice);
    }
}
