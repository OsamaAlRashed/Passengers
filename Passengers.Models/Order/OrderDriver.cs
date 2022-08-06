using Passengers.Models.Base;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderSet = Passengers.Models.Order.Order;

namespace Passengers.Models.Order
{
    public class OrderDriver : BaseEntity
    {
        public Guid OrderId { get; set; }
        public OrderSet Order { get; set; }

        public Guid DriverId { get; set; }
        public AppUser Driver { get; set; }

        public OrderDriverType OrderDriverType { get; set; }
    }
}
