using Passengers.Models.Base;
using Passengers.Models.Security;
using Passengers.Models.Shared;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Passengers.Models.Order
{
    public class Order : BaseEntity
    {
        public Order()
        {
            Rates = new HashSet<Rate>();
            OrderDetails = new HashSet<OrderDetails>();
        }
        
        public OrderStatus OrderStatus { get; set; }
        public long TotalPrice { get; set; }
        public string Note { get; set; }
        public OrderTypes OrderType { get; set; }
        public DateTime? InShopDate { get; set; }
        public DateTime? OnWay { get; set; }
        public DateTime? CloseDate { get; set; }


        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public AppUser Customer { get; set; }

        public Guid? DriverId { get; set; }
        [ForeignKey(nameof(DriverId))]
        public AppUser Driver { get; set; }


        public ICollection<OrderDetails> OrderDetails { get; set; }
        public ICollection<Rate> Rates { get; set; }
    }
}
