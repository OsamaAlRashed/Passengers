using Passengers.Models.Base;
using Passengers.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Main
{
    public class ShopSchedule : BaseEntity
    {
        public string Days { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }

        public Guid ShopId { get; set; }
        public AppUser Shop { get; set; }
    }
}
