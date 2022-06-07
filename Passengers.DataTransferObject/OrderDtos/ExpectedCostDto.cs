using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class ExpectedCostDto
    {
        public int Time { get; set; }
        public int Cost { get; set; }
        public string ShopName { get; set; }
        public string SubTotal { get; set; }
    }
}
