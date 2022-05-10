using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.CustomerDtos
{
    public class CustomerProductFilterDto
    {
        public string Search { get; set; }
        public int? Rate { get; set; }
        public bool? WithDiscount { get; set; }
        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }
        public Guid? TagId { get; set; }
    }
}
