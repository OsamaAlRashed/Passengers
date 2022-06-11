using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.OrderDtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public string ShopName { get; set; }
    }
}
