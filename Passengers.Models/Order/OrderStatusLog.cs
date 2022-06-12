using Passengers.Models.Base;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Order
{
    public class OrderStatusLog : BaseEntity
    {
        public OrderStatus Status { get; set; }
        public string Note { get; set; }

        public Order Order { get; set; }
        public Guid OrderId { get; set; }
    }
}
