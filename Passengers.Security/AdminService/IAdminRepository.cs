using Passengers.DataTransferObject.SecurityDtos;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.Pagnation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.AdminService
{
    public interface IAdminRepository
    {
        Task<OperationResult<PagedList<GetAdminDto>>> Get(int pageNumber, int pageSize, string search);
        Task<OperationResult<GetAdminDto>> GetById(Guid id);
        Task<OperationResult<GetAdminDto>> Add(SetAdminDto dto);
        Task<OperationResult<GetAdminDto>> Update(SetAdminDto dto);
        Task<OperationResult<GetAdminDto>> Delete(Guid id);
        Task<OperationResult<bool>> Block(Guid id);

        Task<OperationResult<PagedList<DashboardShopDto>>> GetShops(int pageNumber, int pageSize, string search);


    }
}
