using Passengers.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.Models.Security
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }

        public Guid UserId { get; set; }
        public AppUser User { get; set; }

        [NotMapped]
        public bool IsExpired => DateCreated.AddDays(7) < DateTime.Now;
    }
}
