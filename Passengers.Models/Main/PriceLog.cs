using Passengers.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Passengers.Models.Main
{
    public class PriceLog : BaseEntity
    {
        public decimal Price { get; set; }
        public DateTime? EndDate { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
