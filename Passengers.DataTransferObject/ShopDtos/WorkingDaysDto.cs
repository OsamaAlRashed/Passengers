using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.DataTransferObject.ShopDtos
{
    public class WorkingDaysDto
    {
        public List<int> Days { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
    }
}
