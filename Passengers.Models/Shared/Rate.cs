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
    public class Rate : BaseEntity
    {
        public int Degree { get; set; }
        public string Descreption { get; set; }

        public Guid CustomerId { get; set; }
        public AppUser Customer { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? OrderId { get; set; }
        public Order.Order Order { get; set; }

    }
}
