using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class DashboardOrderDto : OrderDto
    {
        public DeliveryCompanyOrderStatus Status { get; set; }
        public string ImagePath { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
    }
}
