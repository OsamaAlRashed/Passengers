using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.ProductDtos
{
    public class FilterFoodMenuDto
    {
        public string Search { get; set; }
        public SortProductTypes SortType { get; set; }
        public bool IsDes { get; set; }
        public List<int> Rates { get; set; }
        public List<Guid> TagIds { get; set; }
        public bool? Avilable { get; set; }
    }
}
