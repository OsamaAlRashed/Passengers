using Passengers.DataTransferObject.BaseDtos;
using Passengers.Models.Location;
using Passengers.SharedKernel.Services.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.LocationDtos
{
    public class CityDto : IKey , ISelector<City, CityDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }

        public static Expression<Func<City, CityDto>> Selector { get; set; } = entity => new CityDto() { Id = entity.Id, Name = entity.Name , CountryId = entity.CountryId };
        public static Expression<Func<CityDto, City>> InverseSelector { get; set; } = dto => new City() { Id = dto.Id, Name = dto.Name , CountryId = dto.CountryId };
        public static Action<CityDto, City> AssignSelector { get; set; } = (dto, entity) => { entity.Name = dto.Name; };

    }
}
