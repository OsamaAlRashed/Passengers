using Passengers.DataTransferObject.TagDtos;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Main.TagService
{
    public interface ITagRepository
    {
        Task<OperationResult<GetTagDto>> Add(SetTagDto dto);
        Task<OperationResult<GetTagDto>> Update(SetTagDto dto);
        Task<OperationResult<bool>> Remove(Guid id);
        Task<OperationResult<List<GetTagDto>>> Get(bool isHidden);
        Task<OperationResult<GetTagDto>> GetById(Guid id);
        Task<OperationResult<List<GetTagDto>>> GetPublicTag();
        Task<OperationResult<List<GetTagDto>>> GetByShopId(Guid id);
        Task<OperationResult<bool>> ChangeStatus(Guid id, bool status);
    }
}
