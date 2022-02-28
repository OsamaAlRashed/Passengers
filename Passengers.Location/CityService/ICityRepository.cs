using Passengers.Base;
using Passengers.DataTransferObject.LocationDtos;
using Passengers.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Location.CityService
{
    public interface ICityRepository : IBaseRepository<CityDto>
    {
    }
}
