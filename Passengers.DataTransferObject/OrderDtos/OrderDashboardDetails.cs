using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class OrderDashboardDetails : OrderDto
    {
        public DateTime? CompletedOn { get; set; }
        public DeliveryCompanyOrderStatus Status { get; set; }
        public OrderDetailsDto Details { get; set; }
        public ExpectedCostDto ExpectedCost { get; set; }
        public UserInfoDto Shop { get; set; }
        public UserInfoDto Customer { get; set; }
        public UserInfoDto Driver { get; set; }
    }

    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string PhoneNumber { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string Note { get; set; }
        public string Address { get; set; }
    }
}
