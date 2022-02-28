using Passengers.DataTransferObject.LocationDtos;
using Passengers.Location.LocationService;
using Passengers.Models.Location;
using Passengers.Repository.Base;
using Passengers.SharedKernel.OperationResult;
using Passengers.SqlServer.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Location.CountryService
{
    public class CountryRepository : BaseRepository<Country,CountryDto> , ICountryRepository
    {
        public CountryRepository(PassengersDbContext context):base(context) { }

        public Task<OperationResult<CountryDto>> Add(CountryDto dto) => base.AddAsync(dto);

        public Task<OperationResult<IEnumerable<CountryDto>>> Get() => base.GetAsync();

        public Task<OperationResult<CountryDto>> GetById(Guid id) => base.GetByIdAsync(id);

        public Task<OperationResult<bool>> Remove(Guid id) => base.DeleteAsync(id);

        public Task<OperationResult<CountryDto>> Update(CountryDto dto) => base.UpdateAsync(dto);
    }
}
