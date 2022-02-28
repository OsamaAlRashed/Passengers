using Passengers.DataTransferObject.RateDtos;
using Passengers.Repository.Base;
using Passengers.SharedKernel.OperationResult;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Shared.RateService
{
    public class RateRepository : BaseRepository, IRateRepository
    {
        public RateRepository(PassengersDbContext context):base(context)
        {
        }

        public Task<OperationResult<RateDto>> Add(RateDto rateDto)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<RateDto>> Get()
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<RateDto>> GetByEntityId(Guid entityId)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<RateDto>> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<bool>> Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<RateDto>> Update(RateDto rateDto)
        {
            throw new NotImplementedException();
        }
    }
}
