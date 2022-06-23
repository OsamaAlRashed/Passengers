using Passengers.Models.Base;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderSet = Passengers.Models.Order.Order;
namespace Passengers.Models.Location
{
    public class Address : BaseEntity
    {
        public Address()
        {
            Orders = new HashSet<OrderSet>();
        }

        public string Title { get; set; }
        public string Building { get; set; }
        public string PhoneNumber { get; set; }
        public string Text { get; set; }
        public string Note { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public bool IsActive { get; set; }
        public bool IsCurrentLocation { get; set; }

        public Guid AreaId { get; set; }
        public Area Area { get; set; }

        public Guid? CustomerId { get; set; }
        public AppUser Customer { get; set; }

        public Guid? ShopId { get; set; }
        public AppUser Shop { get; set; }

        public ICollection<OrderSet> Orders { get; set; }

    }
}
