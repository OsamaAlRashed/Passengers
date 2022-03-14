using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.ProductDtos
{
    public class ProductFilterDto
    {
        public string Search { get; set; }
        public List<Guid> TagIds { get; set; }
        public int? Rate { get; set; }
        public bool? Avilable { get; set; }
        public bool? Discount { get; set; }
        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }
    }
}
