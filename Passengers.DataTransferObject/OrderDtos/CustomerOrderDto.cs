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
        public decimal TotalCost 
        {
            get => SubTotal + (DeliveryCost ?? 0);
        }
        public string DriverNote { get; set; }
        public string AddressTitle { get; set; }
        public double Distance { get; set; }
        public int Time { get; set; }
        public Guid CustomerId { get; set; }
        public string ShopLat { get; set; }
        public string ShopLng { get; set; }
        public string CustomerLat { get; set; }
        public string CustomerLng { get; set; }
    }
}
