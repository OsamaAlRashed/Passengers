using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class DriverOrderDetailsDto : DriverOrderDto
    {
        public string CustomerAddress { get; set; }
        public int Time { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal SubTotal { get; set; }
        public double Distance { get; set; }
        public string ShopLat { get; set; }
        public string ShopLng { get; set; }
        public string CustomerLat { get; set; }
        public string CustomerLng { get; set; }
        public decimal TotalCost { get => DeliveryCost + SubTotal; }
    }
}
