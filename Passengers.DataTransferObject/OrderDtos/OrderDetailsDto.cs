using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class OrderDetailsDto
    {
        public List<ProductCardDto> Products { get; set; }
        public string Note { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}
