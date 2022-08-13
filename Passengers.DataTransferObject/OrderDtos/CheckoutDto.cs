using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class CheckoutDto
    {
        public ExpectedCostDto ExpectedCost { get; set; }
        public List<ShopCostDto> ShopCosts { get; set; }
    }
}
