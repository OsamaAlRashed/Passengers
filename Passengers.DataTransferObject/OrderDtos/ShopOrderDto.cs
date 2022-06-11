using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class ShopOrderDto : OrderDto
    {
        public List<ProductCardDto> Products { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
