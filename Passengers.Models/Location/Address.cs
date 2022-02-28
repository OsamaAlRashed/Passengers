using Passengers.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Location
{
    public class Address : BaseEntity
    {
        public string Text { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }

        public Guid AreaId { get; set; }
        public Area Area { get; set; }
    }
}
