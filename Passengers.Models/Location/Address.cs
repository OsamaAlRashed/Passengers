using Passengers.Models.Base;
using Passengers.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Location
{
    public class Address : BaseEntity
    {
        public Address()
        {

        }

        public string Text { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }

        public Guid AreaId { get; set; }
        public Area Area { get; set; }

        public Guid? CustomerId { get; set; }
        public AppUser Customer { get; set; }

        public Guid? ShopId { get; set; }
        public AppUser Shop { get; set; }

    }
}
