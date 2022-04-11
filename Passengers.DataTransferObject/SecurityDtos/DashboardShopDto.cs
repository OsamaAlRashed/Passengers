using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.SecurityDtos
{
    public class DashboardShopDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public string Category { get; set; }
        public bool Online { get; set; }
        public int? FromDay { get; set; }
        public int? ToDay { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public object Contacts { get; set; }
        public object Address { get; set; }
        public Passengers.SharedKernel.Enums.DeliveryShopStatus? DeliveryShopStatus { get; set; }
    }
}
