using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class OrderDashboardDetails
    {
        public OrderDetailsDto Details { get; set; }
        public ExpectedCostDto ExpectedCost { get; set; }
        public DashboardOrderDto MainInfo { get; set; }
    }
}
