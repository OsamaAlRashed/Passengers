using Passengers.DataTransferObject.GeneralDtos;
using Passengers.DataTransferObject.ProductDtos;
using Passengers.Models.Main;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult;
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
        Task<OperationResult<List<GetProductDto>>> Get();
        Task<OperationResult<object>> GetById(Guid id);
        Task<OperationResult<PagnationDto<object>>> GetFoodMenu(Guid tagId, int pageSize, int pageNumber);
        Task<OperationResult<bool>> ChangePrice(Guid id, decimal newPrice);

        //int pageSize, int pageNumber, List<Guid> tagIds, SortProductTypes? type, string search, int? Rate, bool? IsAvilable, decimal? fromPrice, decimal? toPrice)
    }
}
