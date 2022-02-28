using Passengers.Models.Base;
using Passengers.Models.Main;
using System;
using System.Collections.Generic;
using System.Text;

namespace Passengers.Models.Order
{
    public class OrderDetails : BaseEntity
    {
        public int Quantity { get; set; }
        public string Note { get; set; }


        public Order Order { get; set; }
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        
    }
}
