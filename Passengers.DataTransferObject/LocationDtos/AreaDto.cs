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
    public class AreaDto : IKey , ISelector<Area, AreaDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CityId { get; set; }

        public static Expression<Func<Area, AreaDto>> Selector { get; set; } = entity => new AreaDto() { Id = entity.Id, Name = entity.Name , CityId = entity.CityId };
        public static Expression<Func<AreaDto, Area>> InverseSelector { get; set; } = dto => new Area() { Id = dto.Id, Name = dto.Name , CityId = dto.CityId };
        public static Action<AreaDto, Area> AssignSelector { get; set; } = (dto, entity) => { entity.Name = dto.Name; entity.CityId = dto.CityId; };
    }
}
