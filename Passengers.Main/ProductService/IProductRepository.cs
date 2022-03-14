using Passengers.DataTransferObject.GeneralDtos;
using Passengers.DataTransferObject.ProductDtos;
using Passengers.Models.Main;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.Pagnation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        Task<OperationResult<object>> GetById(Guid id);
        Task<OperationResult<PagedList<GetProductDto>>> Get(ProductFilterDto filterDto, SortProductTypes? sortType, bool? isDes, int pageNumber = 1, int pageSize = 10);
        Task<OperationResult<bool>> ChangePrice(Guid id, decimal newPrice);
    }
}
