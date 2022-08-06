using Passengers.Models.Base;
using Passengers.Models.Location;
using Passengers.Models.Security;
using Passengers.Models.Shared;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Passengers.Models.Order
{
    public class Order : BaseEntity
    {
        public Order()
        {
            Reviews = new HashSet<Review>();
            OrderDetails = new HashSet<OrderDetails>();
            OrderStatusLogs = new HashSet<OrderStatusLog>();
            OrderDrivers = new HashSet<OrderDriver>();
        }

        public string SerialNumber { get; set; }
        public string ShopNote { get; set; }
        public bool IsShopReady { get; set; }
        public string DriverNote { get; set; }
        public OrderTypes OrderType { get; set; }
        public decimal? DeliveryCost { get; set; }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            public decimal ExpectedCost { get; set; }
        public int ExpectedTime { get; set; }

        public Guid AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public Address Address { get; set; }

        public Guid? DriverId { get; set; }
        [ForeignKey(nameof(DriverId))]
        public AppUser Driver { get; set; }


        public ICollection<OrderDetails> OrderDetails { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<OrderStatusLog> OrderStatusLogs { get; set; }
        public ICollection<OrderDriver> OrderDrivers { get; set; }

        //[NotMapped] 
        //public OrderStatus Status => OrderStatusLogs.OrderBy(x => x.DateCreated).Select(x => x.Status).LastOrDefault();

    }
}
