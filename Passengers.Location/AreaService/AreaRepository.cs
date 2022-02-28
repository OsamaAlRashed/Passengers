using Passengers.DataTransferObject.LocationDtos;
using Passengers.Models.Location;
using Passengers.Repository.Base;
using Passengers.Shared.SharedService;
using Passengers.SharedKernel.OperationResult;
using Passengers.SharedKernel.OperationResult.Enums;
using Passengers.SharedKernel.OperationResult.ExtensionMethods;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Location.AreaService
{
    public class AreaRepository : BaseRepository<Area,AreaDto> , IAreaRepository
    {
        private readonly ISharedRepository sharedRepository;
        public AreaRepository(PassengersDbContext context, ISharedRepository sharedRepository) : base(context)
        {
            this.sharedRepository = sharedRepository;
        }

        public async Task<OperationResult<AreaDto>> Add(AreaDto dto)
        {
            var isCityExist = await sharedRepository.CheckIsExist<City>(dto.CityId);
            if(isCityExist)
                return _Operation.SetContent<AreaDto>(OperationResultTypes.NotExist, $"CityId {dto.CityId} not Exist");
            return await base.AddAsync(dto);
        }

        public async Task<OperationResult<IEnumerable<AreaDto>>> Get() => await base.GetAsync();

        public async Task<OperationResult<AreaDto>> GetById(Guid id) => await base.GetByIdAsync(id);

        public async Task<OperationResult<bool>> Remove(Guid id) => await base.DeleteAsync(id);

        public async Task<OperationResult<AreaDto>> Update(AreaDto dto)
        {
            var isCityExist = await sharedRepository.CheckIsExist<City>(dto.CityId);
            if (isCityExist)
                return _Operation.SetContent<AreaDto>(OperationResultTypes.NotExist, $"CityId {dto.CityId} not Exist");
            return await base.UpdateAsync(dto);
        }
    }
}
