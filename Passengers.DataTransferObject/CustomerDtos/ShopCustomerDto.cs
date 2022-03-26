using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.CustomerDtos
{
    public class ShopCustomerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public double Rate { get; set; }
        public bool Online { get; set; }
        public string ImagePath { get; set; }
    }
}
