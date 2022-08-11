using Microsoft.AspNetCore.Http;
using Passengers.DataTransferObject.DriverDtos;
using Passengers.DataTransferObject.SecurityDtos;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.Pagnation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.DriveService
{
    public interface IDriverRepository
    {
        Task<OperationResult<PagedList<GetDriverDto>>> Get(int pageNumber, int pageSize, string search, bool? online);
        Task<OperationResult<GetDriverDto>> GetById(Guid id);
        Task<OperationResult<GetDriverDto>> Add(SetDriverDto dto);
        Task<OperationResult<GetDriverDto>> Update(SetDriverDto dto);
        Task<OperationResult<bool>> Delete(Guid id);
        Task<OperationResult<DetailsDriverDto>> Details(Guid id, DateTime? day);
        Task<OperationResult<bool>> ChangeAvilability(bool status, string lat, string lng);
        Task<OperationResult<object>> Login(LoginDriverDto dto);
        Task<OperationResult<string>> UploadImage(IFormFile file);
        Task<OperationResult<GetDriverDto>> GetMyInformations();
        Task<OperationResult<DriverStatisticsDto>> GetStatistics(DateTime? date);

    }
}
