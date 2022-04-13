using Passengers.Models.Base;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Main
{
    public class Payment : BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public PaymentType Type { get; set; }

        public AppUser User { get; set; }
        public Guid? UserId { get; set; }
    }
}
