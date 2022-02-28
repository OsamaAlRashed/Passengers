using Passengers.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Main
{
    public class Discount : BaseEntity
    {
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
