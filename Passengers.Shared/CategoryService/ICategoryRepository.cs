using Passengers.DataTransferObject.SharedDtos.CategoryDtos;
using Passengers.SharedKernel.Enums;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.CategoryService
{
    public interface ICategoryRepository
    {
        Task<OperationResult<GetCategoryDto>> Add(SetCategoryDto dto);
        Task<OperationResult<GetCategoryDto>> Update(SetCategoryDto dto);
        Task<OperationResult<bool>> Remove(Guid id);

        Task<OperationResult<List<GetCategoryDto>>> Get();
        Task<OperationResult<GetCategoryDto>> GetById(Guid id);
        Task<string> GetByShopId(Guid id);
        Task<OperationResult<List<GetCategoryDto>>> GetChildern(Guid id);
        Task<OperationResult<List<GetCategoryDto>>> GetRoots();
        Task<OperationResult<TreeDto>> GetTree(Guid id);
        Task<OperationResult<List<string>>> GetFullPath(Guid id);


    }
}
