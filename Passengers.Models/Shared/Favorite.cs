using Passengers.Models.Base;
using Passengers.Models.Main;
using Passengers.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Shared
{
    public class Favorite : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public AppUser Customer { get; set; }

        public Guid? ShopId { get; set; }
        public AppUser Shop { get; set; }

        public Product Product { get; set; }
        public Guid? ProductId { get; set; }
    }
}
