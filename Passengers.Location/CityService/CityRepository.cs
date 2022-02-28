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

namespace Passengers.Location.CityService
{
    public class CityRepository : BaseRepository<City , CityDto>, ICityRepository
    {
        private readonly ISharedRepository sharedRepository;

        public CityRepository(PassengersDbContext context, ISharedRepository sharedRepository) : base(context)
        {
            this.sharedRepository = sharedRepository;
        }

        public async Task<OperationResult<CityDto>> Add(CityDto dto)
        {
            var isCountryExist = await sharedRepository.CheckIsExist<Country>(dto.CountryId);
            if (isCountryExist)
                return _Operation.SetContent<CityDto>(OperationResultTypes.NotExist, $"CountryId {dto.CountryId} not Exist");
            return await base.AddAsync(dto);
        }

        public async Task<OperationResult<IEnumerable<CityDto>>> Get() => await base.GetAsync();

        public async Task<OperationResult<CityDto>> GetById(Guid id) => await base.GetByIdAsync(id);

        public async Task<OperationResult<bool>> Remove(Guid id) => await base.DeleteAsync(id);

        public async Task<OperationResult<CityDto>> Update(CityDto dto)
        {
            var isCountryExist = await sharedRepository.CheckIsExist<Country>(dto.CountryId);
            if (isCountryExist)
                return _Operation.SetContent<CityDto>(OperationResultTypes.NotExist, $"CountryId {dto.CountryId} not Exist");
            return await base.UpdateAsync(dto);
        }

    }
}
