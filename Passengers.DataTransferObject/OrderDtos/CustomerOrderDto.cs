using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class CustomerOrderDto : OrderDto
    {
        public CustomerOrderStatus Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? DeliveryCost { get; set; }
        public decimal TotalCost { get; set; }
        public string DriverNote { get; set; }
        public string AddressTitle { get; set; }
        public int Distance { get; set; }
        public int Time { get; set; }
    }
}
