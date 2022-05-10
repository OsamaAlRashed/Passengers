using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.DriverDtos
{
    public class DetailsDriverDto : GetDriverDto
    {
        public decimal FixedAmount { get; set; }
        public decimal DeliveryAmount { get; set; }
        public decimal TotalAmount => FixedAmount + DeliveryAmount;
        public int OrderCount { get; set; }
        public int OnlineTime { get; set; }
    }
}
