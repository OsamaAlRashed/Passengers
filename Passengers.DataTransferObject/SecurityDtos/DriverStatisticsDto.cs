using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.SecurityDtos
{
    public class DriverStatisticsDto
    {
        public decimal TotalBalance { get { return FixedAmount + DeliveryAmount; } }
        public int OrderCount { get; set; }
        public decimal FixedAmount { get; set; }
        public decimal DeliveryAmount { get; set; }    
        public double? SpeedAverage { get; set; }    
    }
}
