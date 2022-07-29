using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class DriverOrderDto : OrderDto
    {
        public DriverOrderStatus Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerImagePath { get; set; }
        public string CustomerPhone { get; set; }
    }
}
