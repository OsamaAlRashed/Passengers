using Passengers.DataTransferObject.DriverDtos;
using Passengers.Repository.Base;
using Passengers.SharedKernel.OperationResult;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Security.DriveService
{
    public class DriverRepository : BaseRepository, IDriverRepository
    {
        public DriverRepository(PassengersDbContext context): base(context)
        {

        }

        public Task<OperationResult<GetDriverDto>> Add(SetDriverDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<bool>> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<List<GetDriverDto>>> Get()
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<GetDriverDto>> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<GetDriverDto>> Update(SetDriverDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
