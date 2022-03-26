using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.CustomerDtos
{
    public class CustomerShopFilterDto
    {
        public bool? NearBy { get; set; }
        public List<int> Days { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string Search { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
