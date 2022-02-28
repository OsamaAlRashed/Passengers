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
    public class CountryDto : IKey , ISelector<Country, CountryDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static Expression<Func<Country, CountryDto>> Selector { get; set; } = entity => new CountryDto() { Id = entity.Id, Name = entity.Name };
        public static Expression<Func<CountryDto, Country>> InverseSelector { get; set; } = dto => new Country() { Id = dto.Id, Name = dto.Name };
        public static Action<CountryDto, Country> AssignSelector { get; set; } = (dto, entity) => { entity.Name = dto.Name; };
    }
}
