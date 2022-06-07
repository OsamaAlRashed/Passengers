using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class SetOrderDto
    {
        public List<ResponseCardDto> Cart { get; set; }
        public Guid AddressId { get; set; }
        public string DriverNote { get; set; }
    }
}
