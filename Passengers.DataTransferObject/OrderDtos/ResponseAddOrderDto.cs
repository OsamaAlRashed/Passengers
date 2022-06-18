using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class ResponseAddOrderDto
    {
        public decimal SubTotal { get; set; }
        public List<ShopCostDto> Shops { get; set; }
    }

    public class ShopCostDto
    {
        public string ShopName { get; set; }
        public decimal Cost { get; set; }
    }
}
